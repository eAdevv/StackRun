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
    [SerializeField] private Transform finalPoint;
    [SerializeField] private GameObject _lastPiece;

    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private PlayerState _playerState;

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

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        PlayerState = PlayerState.Idle;
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
        if (!pieceManager.IsPiecePlaced && transform.position.z >= (_lastPiece.transform.position.z + _lastPiece.transform.localScale.z) - 1.25f)
            EventManager.OnGameFail?.Invoke();

    }
    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted && PlayerState == PlayerState.Run && !gameManager.IsGameFnish)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        playerAnimator.SetBool(Runnig,true);
        playerRigidbody.velocity = Vector3.forward * PlayerSpeed;
       
        if (_lastPiece != null)
        {
            var piecePoint = new Vector3(_lastPiece.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, piecePoint, TIME_MULTIPLIER * Time.deltaTime);
        }
    }

    private void PlayerFall()
    {
        playerAnimator.SetBool(Runnig, false);
        playerAnimator.SetTrigger(Fall);
        _playerState = PlayerState.Fail;
        playerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        GetComponent<Collider>().enabled = false;
    }

    private void PlayerFnishActivity(Vector3 fnishPoint)
    {
        gameManager.IsGameFnish = true;
        transform.DOMove(fnishPoint, 2f).OnComplete(() =>
        {
            _playerState = PlayerState.Win;
            playerAnimator.SetBool(Runnig, false);
            playerAnimator.SetTrigger(Dance);
            playerRigidbody.constraints = RigidbodyConstraints.FreezePosition;
            EventManager.OnCameraFnish?.Invoke();
        });

    }

    private void GetLastPiece(GameObject lastPiece)
    {
        _lastPiece = lastPiece;
    }





}
