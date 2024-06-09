using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float FAIL_CANVAS_OPEN_DELAY = 2F;

    [SerializeField] private GameObject StartText;
    [SerializeField] private GameObject FailCanvas;
    [SerializeField] private GameObject WinCanvas;

    private bool isGameStarted;
    private bool isGameFnish;

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

    private void Start()
    {
        FailCanvas.GetComponentInChildren<Button>().onClick.AddListener(GameRestart);
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += GameStart;
        EventManager.OnGameFail += GameFail;
        EventManager.OnGameWin += GameWin;
    }
    private void OnDisable()
    {
        EventManager.OnGameStart -= GameStart;
        EventManager.OnGameFail -= GameFail;
        EventManager.OnGameWin -= GameWin;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartText.SetActive(false);
            EventManager.OnCameraIdleToStart();
        }
    }

    private void GameStart()
    {
        IsGameStarted = true;
        playerManager.PlayerState = PlayerState.Run;
        EventManager.OnSpawnPiece(pieceManager.PiecePrefab.transform.localScale, pieceManager.transform.position); // Ilk parca spawn
    }

    private void GameFail()
    {
        EventManager.OnPlayerFall?.Invoke();
        IsGameFnish = true;
        StartCoroutine(DelayedFailCanvas(FAIL_CANVAS_OPEN_DELAY));
    }

    private void GameWin()
    {
        IsGameFnish = true;
        EventManager.OnPlayerFnishActivity?.Invoke(WinCanvas);
    }

    public void GameRestart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator DelayedFailCanvas(float delay)
    {
        yield return new WaitForSeconds(delay);
        FailCanvas.SetActive(true);
        EventManager.OnCameraStop?.Invoke();

    }
}
