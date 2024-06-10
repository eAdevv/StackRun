using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FnishTriggerController : MonoBehaviour
{
    [SerializeField]private Transform FinalPoint;
    [Inject]
    PlayerManager playerManager;
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.TryGetComponent(out PlayerManager player);
        if (!isPlayer) return;

        EventManager.OnGameWin?.Invoke(FinalPoint);
        EventManager.OnGetLastPiece?.Invoke(gameObject);

        if (gameObject.layer == LayerMask.NameToLayer("FinalPlatform")) playerManager.IsGameEnd = true;
    }
}
