using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public static class EventManager 
{
    #region Game Events
    public static UnityAction OnGameStart;
    public static UnityAction OnGameFail;
    public static UnityAction<GameObject> OnGetLastPiece;
    public static UnityAction<Vector3, Vector3> OnSpawnPiece;
    #endregion

    #region Camera Events
    public static UnityAction OnCameraIdleToStart;
    public static UnityAction OnCameraStop;
    public static UnityAction OnCameraFnish;
    #endregion

    #region Player Events
    public static UnityAction OnPlayerFall;
    public static UnityAction OnPlayerFnishActivity;
    #endregion
}
