using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Flags]
public enum MovementBlockFlag
{
    None = 0,
    PlayerOnly = 1,
    NpcOnly = 2,
    All = 4
}

public class Position : EntityComponent
{
    public MovementBlockFlag BlockMovementOfFlag = MovementBlockFlag.None;
    public Point Point;
    
    float TransitionTime = 20f;

    Vector3 destinationPosition;

    void Start()
    {
        RegisteredEvents.Add(GameEventId.MoveEntity, MoveEntity);
        RegisteredEvents.Add(GameEventId.SetEntityPosition, SetEntityPosition);
        RegisteredEvents.Add(GameEventId.CalculateTileFlags, CalculateTileFlags);

        Point p = new Point((int)transform.position.x, (int)transform.position.y);
        SetGameObjectDestination(p);

        Services.Map.GetTile(p).AddObject(gameObject);
        destinationPosition = transform.position;
    }

    void CalculateTileFlags(GameEvent gameEvent)
    {
        var flags = gameEvent.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        flags |= BlockMovementOfFlag;
        gameEvent.SetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags, flags);
    }

    void SetEntityPosition(GameEvent gameEvent)
    {
        var point = gameEvent.GetValue<Point>(EventParameter.Point);

        Services.Map.GetTile(Point).RemoveObject(gameObject);
        Point = point;
        Services.Map.GetTile(Point).AddObject(gameObject);
        
        transform.position = Services.Map.GetTile(point).transform.position;
        destinationPosition = transform.position;
    }

    void MoveEntity(GameEvent gameEvent)
    {
        bool canMove = gameEvent.GetValue<bool>(EventParameter.CanMove);
        var newPos = gameEvent.GetValue<Point>(EventParameter.TilePosition);

        if (canMove)
            SetGameObjectDestination(newPos);
        else
            StartCoroutine(FailedMoveTo(Point, newPos));
    }

    IEnumerator FailedMoveTo(Point startPoint, Point desiredPoint)
    {
        SetGameObjectDestination(desiredPoint);
        yield return new WaitForSeconds(0.05f);
        SetGameObjectDestination(startPoint);
    }

    void SetGameObjectDestination(Point point)
    {
        Services.Map.GetTile(Point).RemoveObject(gameObject);
        Point = point;
        Services.Map.GetTile(Point).AddObject(gameObject);

        var tilePosition = Services.Map.GetTile(Point).transform.position;
        var newPosition = new Vector3(tilePosition.x, tilePosition.y, transform.position.z);
        destinationPosition = newPosition;

    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, destinationPosition, TransitionTime * Time.deltaTime);
    }
}
