using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum SpawnDirection
{
    Left,
    Right,
}
public class PieceManager : MonoBehaviour
{
    private const float MAX_DIRECTION_DISTANCE_LIMIT = 10;

    [SerializeField] private PlatformPiece PlatformPiecePrefab;
    [SerializeField] private GameObject StartPlatform;
    [SerializeField] private float pieceSpeed;

    private SpawnDirection PieceDirection;
    private bool canSpawn;

    private GameObject LastPiece;
    private GameObject CurrentPiece;
    public bool CanSpawn
    {
        get => canSpawn;
        set => canSpawn = value;
    }
    public float PieceSpeed
    {
        get => pieceSpeed;
        set => pieceSpeed = value;
    }
    private void Awake()
    {
        if (gameObject.transform.position.x > 0) PieceDirection = SpawnDirection.Left; // Spawner Saðda ise piece sola gider;
        else PieceDirection = SpawnDirection.Right; // Deðilse saða gider;

        LastPiece = StartPlatform;
        OnSpawnPiece();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            DOTween.Kill(CurrentPiece.transform);
            LastPiece = CurrentPiece;
            OnSetSpawnerPosition();
        }
    }

    private void OnSpawnPiece()
    {
        CurrentPiece = Instantiate(PlatformPiecePrefab.gameObject, transform.position, Quaternion.identity);
        PieceMovement(CurrentPiece);
    }
    private void PieceMovement(GameObject Piece)
    {
        // Parçanýn gideceði yönü ayarla ve hareket ettir.
        switch (PieceDirection)
        {
            case SpawnDirection.Left:
                Piece.transform.DOMoveX(transform.position.x - MAX_DIRECTION_DISTANCE_LIMIT, PieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;

            case SpawnDirection.Right:
                Piece.transform.DOMoveX(transform.position.x + MAX_DIRECTION_DISTANCE_LIMIT, PieceSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); ;
                break;
        }
    }

    private void OnSetSpawnerPosition()
    {
        if (PieceDirection == SpawnDirection.Right) SpawnerPosition(SpawnDirection.Left);
        else if (PieceDirection == SpawnDirection.Left) SpawnerPosition(SpawnDirection.Right);

    }

    private void SpawnerPosition(SpawnDirection DirectionState)
    {
        var XPosition = transform.position.x;
        transform.position = new Vector3(-XPosition, transform.position.y, transform.position.z + PlatformPiecePrefab.transform.localScale.z);
        PieceDirection = DirectionState;
        OnSpawnPiece();
    }

    private void CutPiece()
    { 
    
    }
}
