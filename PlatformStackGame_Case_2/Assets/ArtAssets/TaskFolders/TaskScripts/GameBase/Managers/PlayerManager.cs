using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

public enum PlayerState
{
    Idle,
    Run,
    Win,
    Fail,
}
public class PlayerManager : MonoBehaviour
{
    private const float TIME_MULTIPLIER = 3;

    [SerializeField] private float _playerSpeed;
    [SerializeField] private List<Transform> levelFnishPoints;
    private int finalPointID;

    private Transform finalPoint;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private PlayerState _playerState;
    private GameObject _lastPiece;
    private bool isPlayerOnFnish;
    private bool isGameEnd;

    private static readonly int Runnig = Animator.StringToHash("Run");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Dance = Animator.StringToHash("Dance");

    [Inject]
    GameManager gameManager;
    [Inject]
    PieceManager pieceManager;
    public float PlayerSpeed
    {
        get => _playerSpeed;
        set => _playerSpeed = value;
    }
    public PlayerState PlayerState
    {
        get => _playerState;
        set => _playerState = value;
    }
    public bool IsGameEnd
    {
        get => isGameEnd;
        set => isGameEnd = value;
    }
    public bool IsPlayerOnFnish
    {
        get => isPlayerOnFnish;
        set => isPlayerOnFnish = value;
    }
    public Transform FinalPoint { get => finalPoint; set => finalPoint = value; }

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        PlayerState = PlayerState.Idle;
        finalPoint = levelFnishPoints[finalPointID];
    }

    private void OnEnable()
    {
        EventManager.OnGetLastPiece += GetLastPiece;
        EventManager.OnPlayerFall += PlayerFall;
        EventManager.OnPlayerFnishActivity += PlayerFnishActivity;
    }
    private void OnDisable()
    {
        EventManager.OnGetLastPiece -= GetLastPiece;
        EventManager.OnPlayerFall -= PlayerFall;
        EventManager.OnPlayerFnishActivity -= PlayerFnishActivity;
    }

    private void Update()
    {
        // Eger parca konulmadýysa fail olur.
        if (!pieceManager.IsPiecePlaced && transform.position.z >= (_lastPiece.transform.position.z + _lastPiece.transform.localScale.z) - 1f)
            EventManager.OnGameFail?.Invoke();

        if (Vector3.Distance(transform.position, FinalPoint.transform.position) < 5f && !IsPlayerOnFnish)
            pieceManager.IsCanSpawn = false;

        SetAnimation();

    }
    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted && PlayerState == PlayerState.Run && !gameManager.IsGameFnish) MovePlayer();
    }

    private void MovePlayer()
    {
        playerRigidbody.velocity = Vector3.forward * PlayerSpeed;

        if (_lastPiece != null)
        {
            var piecePoint = new Vector3(_lastPiece.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, piecePoint, TIME_MULTIPLIER * Time.deltaTime);
        }
    }

    private void SetAnimation()
    {
        switch (PlayerState)
        {
            case PlayerState.Idle:
                playerAnimator.SetBool(Runnig, false);
                playerAnimator.SetBool(Dance, false);
                break;
            case PlayerState.Run:
                playerAnimator.SetBool(Runnig, true);
                break;
            case PlayerState.Fail:
                playerAnimator.SetBool(Runnig, false);
                playerAnimator.SetBool(Fall, true);
                break;
            case PlayerState.Win:
                playerAnimator.SetBool(Runnig, false);
                playerAnimator.SetBool(Dance, true);
                break;
        }

    }

   
    private void PlayerFall()
    {
        _playerState = PlayerState.Fail;
        playerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        GetComponent<Collider>().enabled = false;
    }

    private void PlayerFnishActivity(GameObject winCanvas)
    {
        IsPlayerOnFnish = true;
        playerRigidbody.constraints = playerRigidbody.constraints | RigidbodyConstraints.FreezePositionZ;
        transform.DOMove(FinalPoint.position, 2f).OnComplete(() =>
        {
            if (!IsGameEnd)
                winCanvas.SetActive(true);

            _playerState = PlayerState.Win;
            EventManager.OnCameraFnish?.Invoke();
            FinalPoint.GetComponentInChildren<ParticleSystem>().Play();
            _lastPiece = pieceManager.FnishPlatform;

            finalPoint = levelFnishPoints[finalPointID+1];
            finalPointID++;

            IsPlayerOnFnish = false;

        });

    }

    private void GetLastPiece(GameObject lastPiece)
    {
        _lastPiece = lastPiece;
    }

}
