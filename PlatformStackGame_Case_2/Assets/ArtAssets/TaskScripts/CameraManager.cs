using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Zenject;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera gameCamera;
    [SerializeField] private Transform StartTransform;
    [SerializeField] private Transform CameraTarget;

    [Inject] PlayerManager playerManager;
    [Inject] GameManager gameManager;
    [Inject] PieceManager pieceManager;

    private void OnEnable()
    {
        EventManager.OnCameraIdleToStart += IdleCamera;
    }
    private void OnDisable()
    {
        EventManager.OnCameraIdleToStart -= IdleCamera;
    }
    private void IdleCamera()
    {
        gameCamera.transform.DOMove(StartTransform.position, 1f);
        gameCamera.transform.DORotate(StartTransform.transform.rotation.eulerAngles,1f).OnComplete(()=> StartGameCamera());
    }

    private void StartGameCamera()
    {
        gameManager.IsGameStarted = true;
        gameCamera.Follow = CameraTarget;
        EventManager.OnSpawnPiece(pieceManager.PiecePrefab.transform.localScale ,pieceManager.transform.position); // First Spawn Piece
    }
}
