using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float FAIL_CANVAS_OPEN_DELAY = 2f;

    private bool isGameStarted;
    private bool isGameFnish;


    [Inject] PlayerManager playerManager;
    [Inject] PieceManager pieceManager;
    [Inject] UIManager _UIManager;
    [Inject] EventManager eventManager;

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
        _UIManager.FailCanvas.GetComponentInChildren<Button>().onClick.AddListener(GameRestart);
        _UIManager.WinCanvas.GetComponentInChildren<Button>().onClick.AddListener(NextLevel);
    }

    private void OnEnable()
    {
        eventManager.OnGameStart += GameStart;
        eventManager.OnGameFail += GameFail;
        eventManager.OnGameWin += GameWin;
    }
    private void OnDisable()
    {
        eventManager.OnGameStart -= GameStart;
        eventManager.OnGameFail -= GameFail;
        eventManager.OnGameWin -= GameWin;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsGameStarted)
        {
            _UIManager.StartText.SetActive(false);
            _UIManager.ClickText.SetActive(true);
            eventManager.OnCameraIdleToStart?.Invoke();
        }
    }

    #region Game State

    private void GameStart()
    {
        IsGameStarted = true;
        playerManager.PlayerState = PlayerState.Run;
        eventManager.OnSpawnPiece?.Invoke(pieceManager.PiecePrefab.transform.localScale, pieceManager.transform.position); // Ilk parca spawn

        if ((playerManager.PlayerRigidbody.constraints & RigidbodyConstraints.FreezePositionZ) == RigidbodyConstraints.FreezePositionZ)
        {
            playerManager.PlayerRigidbody.constraints = playerManager.PlayerRigidbody.constraints & ~RigidbodyConstraints.FreezePositionZ;
        }

        pieceManager.IsCanSpawn = true;

        Debug.Log("LEVEL STARTED");

    }
    private void GameFail()
    {
        eventManager.OnPlayerFall?.Invoke();
        IsGameFnish = true;
        StartCoroutine(DelayedFailCanvas(FAIL_CANVAS_OPEN_DELAY));
        _UIManager.ClickText.SetActive(false);

        Debug.Log("GAME FAIL");
    }

    private void GameWin(Transform finalPos)
    {
        IsGameFnish = true;
        _UIManager.ClickText.SetActive(false);
        eventManager.OnPlayerFnishActivity?.Invoke(_UIManager.WinCanvas);

        Debug.Log("LEVEL COMPLETED");
    }

    private void GameRestart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator DelayedFailCanvas(float delay)
    {
        yield return new WaitForSeconds(delay);
        _UIManager.FailCanvas.SetActive(true);
        eventManager.OnCameraStop?.Invoke();
    }

    private void NextLevel()
    {
        NextLevetEventsHolder();
        isGameFnish = false;
        IsGameStarted = false;
        playerManager.PlayerState = PlayerState.Idle;
    }

    private void NextLevetEventsHolder()
    {
        eventManager.OnNextLevelPieceActivity?.Invoke();
        eventManager.OnNextLevelUIChange?.Invoke();
        eventManager.OnCameraStart?.Invoke();
        eventManager.OnCameraFnishPointChange?.Invoke();
    }
    #endregion









}
