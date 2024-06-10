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
    [Inject]
    EventManager eventManager;
   
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
    public Rigidbody PlayerRigidbody { get => playerRigidbody; set => playerRigidbody = value; }

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerState = PlayerState.Idle;
        finalPoint = levelFnishPoints[finalPointID];
    }

    private void OnEnable()
    {
        eventManager.OnGetLastPiece += GetLastPiece;
        eventManager.OnPlayerFall += PlayerFall;
        eventManager.OnPlayerFnishActivity += PlayerFnishActivity;
    }
    private void OnDisable()
    {
        eventManager.OnGetLastPiece -= GetLastPiece;
        eventManager.OnPlayerFall -= PlayerFall;
        eventManager.OnPlayerFnishActivity -= PlayerFnishActivity;
    }

    private void Update()
    {
        // Eger parca konulmadýysa fail olur.
        if (!pieceManager.IsPiecePlaced && transform.position.z >= (_lastPiece.transform.position.z + _lastPiece.transform.localScale.z) - 1f && !gameManager.IsGameFnish)
            eventManager.OnGameFail?.Invoke();

        if (Vector3.Distance(transform.position, finalPoint.transform.position) < 5f && !isPlayerOnFnish)
            pieceManager.IsCanSpawn = false;

        SetAnimation();

    }
    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted && PlayerState == PlayerState.Run && !gameManager.IsGameFnish) MovePlayer();
    }

    private void MovePlayer()
    {
        PlayerRigidbody.velocity = Vector3.forward * _playerSpeed;

        if (_lastPiece != null)
        {
            var piecePoint = new Vector3(_lastPiece.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, piecePoint, TIME_MULTIPLIER * Time.deltaTime);
        }
        else 
            Debug.LogError("LAST PIECE NOT FOUND");
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
        PlayerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        GetComponent<Collider>().enabled = false;
    }

    private void PlayerFnishActivity(GameObject winCanvas)
    {
        isPlayerOnFnish = true;
        PlayerRigidbody.constraints = PlayerRigidbody.constraints | RigidbodyConstraints.FreezePositionZ;
        transform.DOMove(finalPoint.position, 2f).OnComplete(() =>
        {
            if (!IsGameEnd)
                winCanvas.SetActive(true);

            _playerState = PlayerState.Win;
            eventManager.OnCameraFnish?.Invoke();
            finalPoint.GetComponentInChildren<ParticleSystem>().Play();
            _lastPiece = pieceManager.FnishPlatform;

            finalPoint = levelFnishPoints[finalPointID+1];
            finalPointID++;

            isPlayerOnFnish = false;

        });

    }

    private void GetLastPiece(GameObject lastPiece)
    {
        _lastPiece = lastPiece;
    }

}
