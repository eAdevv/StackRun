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

    [SerializeField] private Rigidbody playerRigidbody;
    [Inject] PlayerManager playerManager;
    [Inject] PieceManager pieceManager;
    [Inject] UIManager _UIManager;

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
            _UIManager.StartText.SetActive(false);
            _UIManager.ClickText.SetActive(true);
            EventManager.OnCameraIdleToStart?.Invoke();
        }
    }

    private void GameStart()
    {
        IsGameStarted = true;
        playerManager.PlayerState = PlayerState.Run;
        EventManager.OnSpawnPiece?.Invoke(pieceManager.PiecePrefab.transform.localScale, pieceManager.transform.position); // Ilk parca spawn

        // Eðer yeni levela geciliyor ise playerin Z constraintsindan cikar.
        if ((playerRigidbody.constraints & RigidbodyConstraints.FreezePositionZ) == RigidbodyConstraints.FreezePositionZ)
        {
            playerRigidbody.constraints = playerRigidbody.constraints & ~RigidbodyConstraints.FreezePositionZ;
        }
        pieceManager.IsCanSpawn = true;

    }

    private void GameFail()
    {
        EventManager.OnPlayerFall?.Invoke();
        IsGameFnish = true;
        StartCoroutine(DelayedFailCanvas(FAIL_CANVAS_OPEN_DELAY));
        _UIManager.ClickText.SetActive(false);
    }

    private void GameWin(Transform finalPos)
    {
        IsGameFnish = true;
        _UIManager.ClickText.SetActive(false);
        EventManager.OnPlayerFnishActivity?.Invoke(_UIManager.WinCanvas);
    }

    public void GameRestart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        EventManager.OnNextLevelPieceActivity?.Invoke();
        EventManager.OnNextLevelUIChange?.Invoke();
        EventManager.OnCameraStart?.Invoke();
        isGameFnish = false;
        playerManager.PlayerState = PlayerState.Idle;
    }

    IEnumerator DelayedFailCanvas(float delay)
    {
        yield return new WaitForSeconds(delay);
        _UIManager.FailCanvas.SetActive(true);
        EventManager.OnCameraStop?.Invoke();
    }

  
    
   
    
}
