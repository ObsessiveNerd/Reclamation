using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static System.TimeZoneInfo;

[Flags]
public enum MovementBlockFlag
{
    None = 0,
    PlayerOnly = 1,
    NpcOnly = 2,
    All = 4
}

public class PositionData : ComponentData
{
    public MovementBlockFlag BlockMovementOfFlag = MovementBlockFlag.None;
    public Point Point;
    public float TransitionTime = 20f;
    public Vector3 DestinationPosition;
}


public class Position : EntityComponent
{
    public PositionData Data = new PositionData();

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.MoveEntity, MoveEntity);
        RegisteredEvents.Add(GameEventId.SetEntityPosition, SetEntityPosition);
        RegisteredEvents.Add(GameEventId.CalculateTileFlags, CalculateTileFlags);
    }

    public override void WakeUp(IComponentData data = null)
    {
        if (data != null)
            Data = data as PositionData;
    }

    void SetEntityPosition(GameEvent gameEvent)
    {
        var point = gameEvent.GetValue<Point>(EventParameter.Point);

        Services.Map.GetTile(Data.Point).RemoveObject(gameObject);
        Data.Point = point;
        Services.Map.GetTile(Data.Point).AddObject(gameObject);

        transform.position = Services.Map.GetTile(point).transform.position;
        Data.DestinationPosition = transform.position;
    }

    void MoveEntity(GameEvent gameEvent)
    {
        bool canMove = gameEvent.GetValue<bool>(EventParameter.CanMove);
        var newPos = gameEvent.GetValue<Point>(EventParameter.TilePosition);

        if (canMove)
            SetGameObjectDestination(newPos);
        else
            Services.Coroutine.InvokeCoroutine(FailedMoveTo(Data.Point, newPos));
    }

    void SetGameObjectDestination(Point point)
    {
        Services.Map.GetTile(Data.Point).RemoveObject(gameObject);
        Data.Point = point;
        Services.Map.GetTile(Data.Point).AddObject(gameObject);

        var tilePosition = Services.Map.GetTile(Data.Point).transform.position;
        var newPosition = new Vector3(tilePosition.x, tilePosition.y, transform.position.z);
        Data.DestinationPosition = newPosition;
    }

    IEnumerator FailedMoveTo(Point startPoint, Point desiredPoint)
    {
        SetGameObjectDestination(desiredPoint);
        yield return new WaitForSeconds(0.05f);
        SetGameObjectDestination(startPoint);
    }

    void CalculateTileFlags(GameEvent gameEvent)
    {
        var flags = gameEvent.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        flags |= Data.BlockMovementOfFlag;
        gameEvent.SetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags, flags);
    }

    public override IComponentData GetData()
    {
        return Data;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Data.DestinationPosition, Data.TransitionTime * Time.deltaTime);
    }
}
