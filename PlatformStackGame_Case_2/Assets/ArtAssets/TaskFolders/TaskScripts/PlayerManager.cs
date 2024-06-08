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

    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private GameObject _lastPiece;
    private PlayerState _playerState;

    private static readonly int Runnig = Animator.StringToHash("Run");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Dance = Animator.StringToHash("Dance");

    [Inject]
    GameManager gameManager;
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
    }
    private void OnDisable()
    {
        EventManager.OnGetLastPiece -= GetLastPiece;
        EventManager.OnPlayerFall -= PlayerFall;
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted && PlayerState == PlayerState.Run)
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
        playerRigidbody.constraints = RigidbodyConstraints.None;
        
    }

    private void GetLastPiece(GameObject lastPiece)
    {
        _lastPiece = lastPiece;
    }





}
