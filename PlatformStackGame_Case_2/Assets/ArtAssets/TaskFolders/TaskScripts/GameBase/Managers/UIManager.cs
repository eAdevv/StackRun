using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Inject] EventManager eventManager;

    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject failCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject clickText;

    public GameObject StartText { get => startText; set => startText = value; }
    public GameObject FailCanvas { get => failCanvas; set => failCanvas = value; }
    public GameObject WinCanvas { get => winCanvas; set => winCanvas = value; }
    public GameObject ClickText { get => clickText; set => clickText = value; }

    private void Awake()
    {
        startText.transform.DOScale(1.2f,.25f).SetLoops(-1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        clickText.transform.DOScale(1.2f,.25f).SetLoops(-1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnEnable()
    {
        eventManager.OnNextLevelUIChange += NextLevelUIChanger;
    }

    private void OnDisable()
    {
        eventManager.OnNextLevelUIChange -= NextLevelUIChanger;
    }
    private void NextLevelUIChanger()
    {
        startText.SetActive(true);
        winCanvas.SetActive(false);
        clickText.SetActive(false);
    }
}
