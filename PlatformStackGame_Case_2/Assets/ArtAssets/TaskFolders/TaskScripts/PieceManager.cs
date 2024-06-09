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

    private const float MAX_DIRECTION_DISTANCE_LIMIT = 10;
    private const float CLICK_TOLERANCE = .9f;

    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject StartPlatform;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private Material[] myColors;
    [SerializeField] private float pieceSpeed;

    private int perfectClickCount;

    private SpawnDirection PieceDirection;

    private GameObject LastPiece;
    private GameObject CurrentPiece;
    private AudioSource audioSource;

    private bool isPiecePlaced;
  
    public GameObject PiecePrefab { get => piecePrefab; set => piecePrefab = value; }
    public float PieceSpeed { get => pieceSpeed; set => pieceSpeed = value; }
    public bool IsPiecePlaced { get => isPiecePlaced; set => isPiecePlaced = value; }

    private void Awake()
    {
        if (gameObject.transform.position.x > 0) PieceDirection = SpawnDirection.Left; // Spawner Saðda ise piece sola gider;
        else PieceDirection = SpawnDirection.Right; // Deðilse saða gider;
    }

    private void Start()
    {
        LastPiece = StartPlatform;
        EventManager.OnGetLastPiece?.Invoke(LastPiece);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.OnSpawnPiece += OnSpawnPiece;  
    }
    private void OnDisable()
    {
        EventManager.OnSpawnPiece -= OnSpawnPiece;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && gameManager.IsGameStarted)
        {
            CutPiece();
        }
    }

    #region Piece Spawn
    private void OnSpawnPiece(Vector3 LocalScale , Vector3 Position)
    {
        if (!gameManager.IsGameFnish)
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
                Piece.transform.DOMoveX(transform.position.x - MAX_DIRECTION_DISTANCE_LIMIT, PieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;

            case SpawnDirection.Right:
                Piece.transform.DOMoveX(transform.position.x + MAX_DIRECTION_DISTANCE_LIMIT, PieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); ;
                break;
        }
    }

    #endregion

    #region Piece Cut

    // Kesile parca 0 dan oluþturulur.
    // Ana parcanýn scaleinden son platform parçasýnýn x pozisyonu ve ana parçanýn x pozisyon deðeri cýkarýlarak yeni parcanýn scale oraný bulunur.
    private void CutPiece()
    {
        GameObject CuttedPiece = Instantiate(PiecePrefab, CurrentPiece.transform.position,Quaternion.identity);
        CuttedPiece.GetComponent<MeshRenderer>().material = CurrentPiece.GetComponent<MeshRenderer>().material;

        Vector3 targetPos = LastPiece.transform.position;
        Vector3 mainScale = CurrentPiece.transform.localScale;
        Vector3 mainPos = CurrentPiece.transform.position;
        CuttedPiece.transform.position = new Vector3((targetPos.x + mainPos.x) / 2f, mainPos.y, mainPos.z);
        CuttedPiece.transform.localScale = new Vector3(CurrentPiece.transform.localScale.x-Mathf.Abs(LastPiece.transform.position.x - CurrentPiece.transform.position.x), CurrentPiece.transform.localScale.y, CurrentPiece.transform.localScale.z);
        var offset = SetPiecePositionOffset(mainPos, targetPos);

        SetCurrentBlock(mainPos, targetPos, mainScale, CuttedPiece, offset);
        RigidbodyChanges(CurrentPiece);
        DOTween.Kill(CurrentPiece.transform);
        // Perfect Click 
        var isPerfectClick = CuttedPiece.transform.localScale.x / CurrentPiece.transform.localScale.x > CLICK_TOLERANCE;

        if (isPerfectClick)
        {
            Debug.Log("Perfect");
            perfectClickCount++;
            audioSource.pitch = 0.5f + (perfectClickCount * .1f);
        }
        else
        {
            Debug.Log("UnPerfect");
            perfectClickCount = 0;
            audioSource.pitch = 0.5f;
        }

        audioSource.PlayOneShot(clickAudio);

        // Fail check
        if (CuttedPiece.transform.localScale.x > .05f)
        {
            IsPiecePlaced = true;
            OnSetSpawnerPosition();
            OnSpawnPiece(CuttedPiece.transform.localScale, transform.position);
            LastPiece = CuttedPiece;
            EventManager.OnGetLastPiece?.Invoke(LastPiece);
        }
        else
        {
            CuttedPiece.transform.position = mainPos;
            CuttedPiece.transform.localScale = LastPiece.transform.localScale;
            RigidbodyChanges(CuttedPiece);
            Destroy(CurrentPiece);

            DOVirtual.DelayedCall(.35f, () => {
                EventManager.OnGameFail?.Invoke();
            });
        }

       
    }
   
    private void SetCurrentBlock(Vector3 MainPos, Vector3 TargetPos, Vector3 MainScale, GameObject cutBlock, float offset)
    {
        CurrentPiece.transform.position = new Vector3((TargetPos.x + MainPos.x) / 2f + MainScale.x * offset / 2f, MainPos.y, MainPos.z);
        CurrentPiece.transform.localScale = new Vector3((LastPiece.transform.localScale.x - cutBlock.transform.localScale.x), LastPiece.transform.localScale.y, LastPiece.transform.localScale.z);
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





}
