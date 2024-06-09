using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private PieceManager pieceManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerManager playerManager;
    public override void InstallBindings()
    {
        Container.Bind<PieceManager>().FromInstance(pieceManager);
        Container.Bind<GameManager>().FromInstance(gameManager);
        Container.Bind<PlayerManager>().FromInstance(playerManager);
    }
}
