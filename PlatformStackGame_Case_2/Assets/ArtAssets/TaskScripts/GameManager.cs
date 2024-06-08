using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class GameManager : MonoBehaviour
{
    private bool isGameStarted;
    [SerializeField] private GameObject StartText;

    public bool IsGameStarted 
    { 
        get => isGameStarted; 
        set => isGameStarted = value; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartText.SetActive(false);
            EventManager.OnCameraIdleToStart();
            
        }
    }
}
