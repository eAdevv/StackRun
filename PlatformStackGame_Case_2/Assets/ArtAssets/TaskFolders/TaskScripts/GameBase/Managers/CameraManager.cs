using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Zenject;


public class CameraManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera StartCamera;
    [SerializeField] private CinemachineVirtualCamera GameCamera;
    [SerializeField] private CinemachineVirtualCamera FnishCamera;
    [SerializeField] private CinemachineDollyCart FnishCameraDolyCart;
    [SerializeField] private GameObject FnishCameraPathHolder;

    [Header("Points")]
    [SerializeField] private Transform StartTransform;
    [SerializeField] private Transform CameraTarget;

    [SerializeField] private List<Transform> fnishCameraPoints = new List<Transform>();
    private Transform currentFnishCameraTransfom;

    [Inject]
    EventManager eventManager;

    private void OnEnable()
    {
        eventManager.OnCameraIdleToStart += IdleCamera;
        eventManager.OnCameraStop += StopGameCamera;
        eventManager.OnCameraFnish += FnishGameCamera;
        eventManager.OnCameraStart += StartGameCamera;
        eventManager.OnCameraFnishPointChange += FnishCameraPointChange;
    }
    private void OnDisable()
    {
        eventManager.OnCameraIdleToStart -= IdleCamera;
        eventManager.OnCameraStop -= StopGameCamera;
        eventManager.OnCameraFnish -= FnishGameCamera;
        eventManager.OnCameraStart -= StartGameCamera;
        eventManager.OnCameraFnishPointChange -= FnishCameraPointChange;
    }

    private void Start()
    {
        currentFnishCameraTransfom = fnishCameraPoints[0];
        currentFnishCameraTransfom.position = fnishCameraPoints[0].position;
        FnishCameraPathHolder.transform.position = currentFnishCameraTransfom.position;
    }
    private void IdleCamera()
    {
        if (FnishCamera.Priority <= StartCamera.Priority)
        {
            StartCamera.Priority = GameCamera.Priority + 1;
            StartCamera.transform.DOMove(StartTransform.position, 1f);
            StartCamera.transform.DORotate(StartTransform.transform.rotation.eulerAngles, 1f).OnComplete(() => StartGameCamera());
        }
        else
        {
            StartGameCamera();
        }
    }
    private void StartGameCamera()
    {
        if (FnishCamera.Priority > GameCamera.Priority)
        {
            GameCamera.Priority = FnishCamera.Priority + 1;
            GameCamera.Follow = CameraTarget;
            StartCamera.enabled = false;
        }
        else
        {
            StartCamera.Priority = GameCamera.Priority - 1;
        }
        eventManager.OnGameStart?.Invoke();
    }
    private void StopGameCamera()
    {
        GameCamera.Follow = null;
    }

    private void FnishGameCamera()
    {
        StopGameCamera();
        FnishCamera.Priority = GameCamera.Priority + 1;
        FnishCameraDolyCart.m_Speed = 5f;
    }

    private void FnishCameraPointChange()
    {
        var currentCamera = fnishCameraPoints.IndexOf(currentFnishCameraTransfom);
        var newCameraPoint = fnishCameraPoints.Find(X => X == fnishCameraPoints[currentCamera + 1]);
        FnishCameraPathHolder.transform.position = newCameraPoint.position;
        currentFnishCameraTransfom = fnishCameraPoints[currentCamera + 1];

    }
}
