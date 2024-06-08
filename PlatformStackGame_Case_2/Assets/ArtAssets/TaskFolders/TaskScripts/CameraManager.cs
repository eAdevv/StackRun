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

    private void OnEnable()
    {
        EventManager.OnCameraIdleToStart += IdleCamera;
        EventManager.OnCameraStop += StopGameCamera;
    }
    private void OnDisable()
    {
        EventManager.OnCameraIdleToStart -= IdleCamera;
        EventManager.OnCameraStop -= StopGameCamera;
    }
    private void IdleCamera()
    {
        gameCamera.transform.DOMove(StartTransform.position, 1f);
        gameCamera.transform.DORotate(StartTransform.transform.rotation.eulerAngles,1f).OnComplete(()=> StartGameCamera());
    }

    private void StartGameCamera()
    {
        gameCamera.Follow = CameraTarget;
        EventManager.OnGameStart?.Invoke();
    }
    private void StopGameCamera()
    {
        gameCamera.Follow = null;
    }
}
