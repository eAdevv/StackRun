using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private PieceManager pieceManager;
    public override void InstallBindings()
    {
        Container.Bind<PieceManager>().FromInstance(pieceManager);
    }
}
