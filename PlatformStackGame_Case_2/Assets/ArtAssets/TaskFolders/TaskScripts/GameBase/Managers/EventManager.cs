using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class EventManager 
{
    #region Game Events

    public UnityAction OnGameStart;
    public UnityAction OnGameFail;
    public UnityAction<Transform> OnGameWin;
    public UnityAction OnNextLevelPieceActivity;
    public UnityAction<GameObject> OnGetLastPiece;
    public UnityAction<Vector3, Vector3> OnSpawnPiece;

    #endregion

    #region Camera Events

    public UnityAction OnCameraIdleToStart;
    public UnityAction OnCameraStop;
    public UnityAction OnCameraFnish;
    public UnityAction OnCameraStart;
    public UnityAction OnCameraFnishPointChange;

    #endregion

    #region Player Events

    public UnityAction OnPlayerFall;
    public UnityAction<GameObject> OnPlayerFnishActivity;

    #endregion

    #region UI Events

    public UnityAction OnNextLevelUIChange;

    #endregion
}
