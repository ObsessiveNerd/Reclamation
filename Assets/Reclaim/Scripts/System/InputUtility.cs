using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtility
{
    public static MoveDirection GetMoveDirection()
    {
        //Need to rework this so we can eventually re-map controlls but this works for now
        MoveDirection direction = MoveDirection.None;

        if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveN))
            direction = MoveDirection.N;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveNE))
            direction = MoveDirection.NE;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveE))
            direction = MoveDirection.E;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveSE))
            direction = MoveDirection.SE;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveS))
            direction = MoveDirection.S;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveSW))
            direction = MoveDirection.SW;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveW))
            direction = MoveDirection.W;
        else if (GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveNW))
            direction = MoveDirection.NW;

        return direction;
    }

    public static MoveDirection GetRandomMoveDirection()
    {
        int randomDirection = RecRandom.Instance.GetRandomValue(0, Enum.GetNames(typeof(MoveDirection)).Length - 1);
        return (MoveDirection)randomDirection;
    }
}
