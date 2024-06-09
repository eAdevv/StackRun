using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Zenject;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera StartCamera;
    [SerializeField] private CinemachineVirtualCamera GameCamera;
    [SerializeField] private Transform StartTransform;
    [SerializeField] private Transform CameraTarget;
    [SerializeField] private Transform Test;

    private void OnEnable()
    {
        EventManager.OnCameraIdleToStart += IdleCamera;
        EventManager.OnCameraStop += StopGameCamera;
        EventManager.OnCameraFnish += FnishGameCamera;
    }
    private void OnDisable()
    {
        EventManager.OnCameraIdleToStart -= IdleCamera;
        EventManager.OnCameraStop -= StopGameCamera;
        EventManager.OnCameraFnish -= FnishGameCamera;
    }
    private void IdleCamera()
    {
        StartCamera.Priority = GameCamera.Priority + 1;
        StartCamera.transform.DOMove(StartTransform.position, 1f);
        StartCamera.transform.DORotate(StartTransform.transform.rotation.eulerAngles,1f).OnComplete(()=> StartGameCamera());
    }

    private void StartGameCamera()
    {
        //gameCamera.Follow = CameraTarget;
        StartCamera.Priority = GameCamera.Priority - 1;
        EventManager.OnGameStart?.Invoke();
    }
    private void StopGameCamera()
    {
        GameCamera.Follow = null;
        GameCamera.LookAt = null;
    }

    private void FnishGameCamera()
    {
        StopGameCamera();
    }
}
