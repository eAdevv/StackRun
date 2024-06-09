using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FnishTriggerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.TryGetComponent(out PlayerManager player);
        if (!isPlayer) return;

        EventManager.OnGameWin?.Invoke();
    }
}
