using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
public enum SpawnDirection
{
    Left,
    Right,
}
public class PieceManager : MonoBehaviour
{
    [Inject] GameManager gameManager;
    [Inject] PlayerManager playerManager;
    [Inject] EventManager eventManager;

    private const float MAX_DIRECTION_DISTANCE_LIMIT = 10;
    private const float CLICK_TOLERANCE = .9f;
    private const float NEXTLEVEL_SPAWNER_POS_OFFSET = 1.35F;

    [Header("Piece Settings")]
    [SerializeField] private float pieceSpeed;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Material[] myColors;

    [Header("Platform Settings")]
    [SerializeField] private GameObject StartPlatform;
    [SerializeField] private List<GameObject> fnishPlatforms = new List<GameObject>();

    [Header("Audio")]
    [SerializeField] private AudioClip perfectClickAudio;
    [SerializeField] private AudioClip notPerfectClickAudio;

    private SpawnDirection PieceDirection;
    private List<GameObject> Pieces = new List<GameObject>();
    private GameObject LastPiece;
    private GameObject CurrentPiece;
    private GameObject fnishPlatform;
    private AudioSource audioSource;

    private bool isPiecePlaced;
    private bool isCanSpawn;
    private int perfectClickCount;
    private int platformIndex;

    public GameObject PiecePrefab { get => piecePrefab; set => piecePrefab = value; }
  
    public bool IsPiecePlaced { get => isPiecePlaced; set => isPiecePlaced = value; }
    public bool IsCanSpawn { get => isCanSpawn; set => isCanSpawn = value; }
    public GameObject FnishPlatform { get => fnishPlatform; set => fnishPlatform = value; }

    private void Awake()
    {
        isCanSpawn = true;

        if (gameObject.transform.position.x > 0) PieceDirection = SpawnDirection.Left; // Spawner Saðda ise piece sola gider;
        else PieceDirection = SpawnDirection.Right; // Deðilse saða gider;
    }

    private void Start()
    {
        LastPiece = StartPlatform;
        eventManager.OnGetLastPiece?.Invoke(LastPiece);
        audioSource = GetComponent<AudioSource>();
        fnishPlatform = fnishPlatforms[0];
    }

    private void OnEnable()
    {
        eventManager.OnSpawnPiece += OnSpawnPiece;
        eventManager.OnNextLevelPieceActivity += NextLevelActivity;  
    }
    private void OnDisable()
    {
        eventManager.OnSpawnPiece -= OnSpawnPiece;
        eventManager.OnNextLevelPieceActivity -= NextLevelActivity;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && gameManager.IsGameStarted && !gameManager.IsGameFnish)
        {
            CutPiece();
        }
    }

    #region Piece Spawn
    private void OnSpawnPiece(Vector3 LocalScale , Vector3 Position)
    {
        if (IsCanSpawn)
        {
            var newPiece = SpawnNewPiece(LocalScale, Position);
            newPiece.GetComponent<MeshRenderer>().material = myColors[Random.Range(0, myColors.Length - 1)];
            PieceMovement(newPiece);
            CurrentPiece = newPiece;
            IsPiecePlaced = false;
        }
    }

    // Yeni gelecek parça ile bir önceki parçanýn scalini  ayný yapar
    private GameObject SpawnNewPiece(Vector3 Scale,Vector3 Pos) 
    {
        GameObject newBlock = Instantiate(PiecePrefab, Pos, Quaternion.identity);
        newBlock.transform.localScale = Scale;
        return newBlock;
    }

    private void OnSetSpawnerPosition()
    {
        if (PieceDirection == SpawnDirection.Right) SpawnerPosition(SpawnDirection.Left);
        else if (PieceDirection == SpawnDirection.Left) SpawnerPosition(SpawnDirection.Right);
    }


    private void SpawnerPosition(SpawnDirection DirectionState)
    {
        var XPosition = transform.position.x;
        transform.position = new Vector3(-XPosition, transform.position.y, transform.position.z + PiecePrefab.transform.localScale.z);
        PieceDirection = DirectionState;
    }

    #endregion

    #region PieceMovement
    private void PieceMovement(GameObject Piece)
    {
        // Parçanýn gideceði yönü ayarla ve hareket ettir.
        switch (PieceDirection)
        {
            case SpawnDirection.Left:
                Piece.transform.DOMoveX(transform.position.x - MAX_DIRECTION_DISTANCE_LIMIT, pieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;

            case SpawnDirection.Right:
                Piece.transform.DOMoveX(transform.position.x + MAX_DIRECTION_DISTANCE_LIMIT, pieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); ;
                break;
        }
    }

    #endregion

    #region Piece Cut

    // Kesile parca 0 dan oluþturulur.
    // Ana parcanýn scaleinden son platform parçasýnýn x pozisyonu ve ana parçanýn x pozisyon deðeri cýkarýlarak yeni parcanýn scale oraný bulunur.
    private void CutPiece()
    {
        // Cut pÝECE
        GameObject CuttedPiece = Instantiate(PiecePrefab, CurrentPiece.transform.position,Quaternion.identity);
        CuttedPiece.GetComponent<MeshRenderer>().material = CurrentPiece.GetComponent<MeshRenderer>().material;

        Vector3 targetPos = LastPiece.transform.position;
        Vector3 mainScale = CurrentPiece.transform.localScale;
        Vector3 mainPos = CurrentPiece.transform.position;
        CuttedPiece.transform.position = new Vector3((targetPos.x + mainPos.x) / 2f, mainPos.y, mainPos.z);
        CuttedPiece.transform.localScale = new Vector3(mainScale.x-Mathf.Abs(targetPos.x - mainPos.x), mainScale.y, mainScale.z);
        var offset = SetPiecePositionOffset(mainPos, targetPos);
        DOTween.Kill(CurrentPiece.transform);

        // Perfect Click 
        var isPerfectClick = PefectClickCheck(CuttedPiece.transform);

        if (isPerfectClick)
        {
            perfectClickCount++;
            audioSource.pitch = 0.5f + (perfectClickCount * .1f);
            audioSource.PlayOneShot(perfectClickAudio);

            SetPerfectCuttedPiece(CuttedPiece, LastPiece.transform);
            Destroy(CurrentPiece);

            Debug.Log("Perfect Click");
        }
        else
        {
            perfectClickCount = 0;
            audioSource.pitch = 0.5f;
            audioSource.PlayOneShot(notPerfectClickAudio);

            SetCurrentBlock(mainPos, targetPos, mainScale, CuttedPiece, offset);
            RigidbodyChanges(CurrentPiece);
            Destroy(CurrentPiece, 2f);
        }


        // Fail check
        if (CuttedPiece.transform.localScale.x > .05f)
        {
            IsPiecePlaced = true;
            OnSetSpawnerPosition();
            OnSpawnPiece(CuttedPiece.transform.localScale, transform.position);
            LastPiece = CuttedPiece;
            Pieces.Add(CuttedPiece);
            eventManager.OnGetLastPiece?.Invoke(LastPiece);
        }
        else
        {
            CuttedPiece.transform.position = mainPos;
            CuttedPiece.transform.localScale = LastPiece.transform.localScale;
            RigidbodyChanges(CuttedPiece);
            Destroy(CurrentPiece);

            StartCoroutine(FailEventDelay(0.5f));
        }

    }

    private bool PefectClickCheck(Transform CuttedPieceTransform)
    { 
        return CuttedPieceTransform.transform.localScale.x / LastPiece.transform.localScale.x > CLICK_TOLERANCE;
    }
   
    private void SetCurrentBlock(Vector3 MainPos, Vector3 TargetPos, Vector3 MainScale, GameObject cutBlock, float offset)
    {
        CurrentPiece.transform.position = new Vector3((TargetPos.x + MainPos.x) / 2f + MainScale.x * offset / 2f, MainPos.y, MainPos.z);
        CurrentPiece.transform.localScale = new Vector3((LastPiece.transform.localScale.x - cutBlock.transform.localScale.x), LastPiece.transform.localScale.y, LastPiece.transform.localScale.z);
    }

    private void SetPerfectCuttedPiece(GameObject cuttedPiece, Transform lastPiece)
    {
        cuttedPiece.transform.localScale = new Vector3(lastPiece.localScale.x, lastPiece.localScale.y, lastPiece.localScale.z);
        cuttedPiece.transform.position = new Vector3(lastPiece.position.x, lastPiece.position.y, cuttedPiece.transform.position.z);
    }

    private void RigidbodyChanges(GameObject cuttedPiece)
    {
        cuttedPiece.AddComponent<Rigidbody>();
        cuttedPiece.GetComponent<Collider>().enabled = false;

    }

    // Parça pozisyon düzeltmesi
    private float SetPiecePositionOffset(Vector3 blockPos, Vector3 targetPos)
    {
        float offset = 0;
        if (blockPos.x - targetPos.x > 0)
        {
            offset = 1; // Parça Saðda
        }
        else
        {
            offset = -1; // Parça SOlda
        }

        return offset;
    }

    #endregion

    #region Level
    private void NextLevelActivity()
    {


        FnishPlatform.GetComponent<MeshRenderer>().enabled = true;
        LastPiece = FnishPlatform;
        perfectClickCount = 0;
        audioSource.pitch = 0.5f;

        if (platformIndex < fnishPlatforms.Count)
        {
            FnishPlatform = fnishPlatforms[platformIndex + 1];
            platformIndex++;
        }

        foreach (var piece in Pieces)
        {
            Destroy(piece);
        }

        Pieces.Clear();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + piecePrefab.transform.localScale.z * NEXTLEVEL_SPAWNER_POS_OFFSET);

    }

    IEnumerator FailEventDelay(float offset)
    {
        yield return new WaitForSeconds(offset);
        eventManager.OnGameFail?.Invoke();
    }

    #endregion

}
