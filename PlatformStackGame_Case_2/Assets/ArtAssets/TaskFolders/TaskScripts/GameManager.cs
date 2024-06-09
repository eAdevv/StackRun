using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float FAIL_CANVAS_OPEN_DELAY = 2F;
    private bool isGameStarted;
    [SerializeField]private bool isGameFnish;
    [SerializeField] private GameObject StartText;
    [SerializeField] private GameObject FailCanvas;
    [SerializeField] private GameObject WinCanvas;
        

    [Inject] PlayerManager playerManager;
    [Inject] PieceManager pieceManager;

    public bool IsGameStarted 
    { 
        get => isGameStarted; 
        set => isGameStarted = value; 
    }
    public bool IsGameFnish
    { 
        get => isGameFnish;
        set => isGameFnish = value;
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
        EventManager.OnPlayerFall?.Invoke();
        IsGameFnish = true;
        StartCoroutine(DelayedFailCanvas(FAIL_CANVAS_OPEN_DELAY));
    }

    public void GameRestart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        FailCanvas.GetComponentInChildren<Button>().onClick.AddListener(GameRestart);
    }

    IEnumerator DelayedFailCanvas(float delay)
    {
        yield return new WaitForSeconds(delay);
        FailCanvas.SetActive(true);
        EventManager.OnCameraStop?.Invoke();

    }
}
