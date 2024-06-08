using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class GameManager : MonoBehaviour
{
    private bool isGameStarted;
    private bool isGameFnish;
    [SerializeField] private GameObject StartText;

    [Inject] PlayerManager playerManager;
    [Inject] PieceManager pieceManager;

    public bool IsGameStarted 
    { 
        get => isGameStarted; 
        set => isGameStarted = value; 
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += GameStart;
        EventManager.OnGameFail += GameFail;
    }
    private void OnDisable()
    {
        EventManager.OnGameStart -= GameStart;
        EventManager.OnGameFail -= GameFail;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartText.SetActive(false);
            EventManager.OnCameraIdleToStart();
        }
    }

    public void GameStart()
    {
        IsGameStarted = true;
        playerManager.PlayerState = PlayerState.Run;
        EventManager.OnSpawnPiece(pieceManager.PiecePrefab.transform.localScale, pieceManager.transform.position); // Ilk parca spawn
    }

    public void GameFail()
    {
        EventManager.OnCameraStop?.Invoke();
        EventManager.OnPlayerFall?.Invoke();
        isGameFnish = true;
    }
}
