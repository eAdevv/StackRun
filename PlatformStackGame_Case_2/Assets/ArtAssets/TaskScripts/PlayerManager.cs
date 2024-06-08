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

    [SerializeField] private float playerSpeed;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private GameObject _lastPiece;

    [Inject]
    GameManager gameManager;
    public float PlayerSpeed
    { 
        get => playerSpeed; 
        set => playerSpeed = value; 
    }

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        EventManager.OnGetLastPiece += GetLastPiece;
    }
    private void OnDisable()
    {
        EventManager.OnGetLastPiece -= GetLastPiece;
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted) MovePlayer();
    }

    private void MovePlayer()
    {
        playerRigidbody.velocity = Vector3.forward * PlayerSpeed;
        var piecePoint = new Vector3(_lastPiece.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, piecePoint ,TIME_MULTIPLIER * Time.deltaTime);
        
    }

    private void GetLastPiece(GameObject lastPiece)
    {
        _lastPiece = lastPiece;
    }





}
