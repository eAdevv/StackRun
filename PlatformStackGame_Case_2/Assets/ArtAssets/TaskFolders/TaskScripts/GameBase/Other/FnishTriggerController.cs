using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FnishTriggerController : MonoBehaviour
{
    [SerializeField]private Transform FinalPoint;
    [Inject]
    PlayerManager playerManager;
    [Inject]
    EventManager eventManager;
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.TryGetComponent(out PlayerManager player);
        if (!isPlayer) return;

        eventManager.OnGameWin?.Invoke(FinalPoint);
        eventManager.OnGetLastPiece?.Invoke(gameObject);

        if (gameObject.layer == LayerMask.NameToLayer("Game_FinalPlatform")) 
            playerManager.IsGameEnd = true;
    }
}
