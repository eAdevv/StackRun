using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class PlayerTriggerController : MonoBehaviour
{
    [Inject] PieceManager pieceManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Border") && !pieceManager.IsPiecePlaced)
        {
            
        }
    }
}
