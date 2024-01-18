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

public class PositionData : EntityComponent
{
    [HideInInspector]
    public Type BehaviorType;
    [HideInInspector]
    public Vector3 DestinationPosition;

    public MovementBlockFlag BlockMovementOfFlag = MovementBlockFlag.None;
    public Point Point;
    public float TransitionTime = 20f;

    public Action<GameEvent> MoveEntity;
    public Action<GameEvent> SetEntityPosition;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.MoveEntity, OnMoveEntity);
        RegisteredEvents.Add(GameEventId.SetEntityPosition, OnSetEntityPosition);
        RegisteredEvents.Add(GameEventId.CalculateTileFlags, CalculateTileFlags);
    }

    void OnMoveEntity(GameEvent gameEvent)
    {
        if (MoveEntity != null)
            MoveEntity(gameEvent);
    }

    void OnSetEntityPosition(GameEvent gameEvent)
    {
        if (SetEntityPosition != null)
            SetEntityPosition(gameEvent);
    }

    void CalculateTileFlags(GameEvent gameEvent)
    {
        var flags = gameEvent.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        flags |= BlockMovementOfFlag;
        gameEvent.SetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags, flags);
    }
}


public class Position : EntityComponentBehavior
{
    public PositionData Data = new PositionData();

    void Start()
    {
        Data.MoveEntity += MoveEntity;
        Data.SetEntityPosition += SetEntityPosition;
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

    IEnumerator FailedMoveTo(Point startPoint, Point desiredPoint)
    {
        SetGameObjectDestination(desiredPoint);
        yield return new WaitForSeconds(0.05f);
        SetGameObjectDestination(startPoint);
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

    void SetEntityPosition(GameEvent gameEvent)
    {
        var point = gameEvent.GetValue<Point>(EventParameter.Point);

        Services.Map.GetTile(Data.Point).RemoveObject(gameObject);
        Data.Point = point;
        Services.Map.GetTile(Data.Point).AddObject(gameObject);

        transform.position = Services.Map.GetTile(point).transform.position;
        Data.DestinationPosition = transform.position;
    }

    public override IComponent GetData()
    {
        return Data;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Data.DestinationPosition, Data.TransitionTime * Time.deltaTime);
    }
}
