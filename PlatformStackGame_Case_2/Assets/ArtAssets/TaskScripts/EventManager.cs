using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager 
{
    public static Action<GameObject> OnGetLastPiece;
    public static Action OnCameraIdleToStart;
    public static Action<Vector3, Vector3> OnSpawnPiece;
}
