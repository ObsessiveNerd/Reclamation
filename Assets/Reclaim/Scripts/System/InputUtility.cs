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

        if (InputBinder.PerformRequestedAction(RequestedAction.MoveN))
            direction = MoveDirection.N;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveNE))
            direction = MoveDirection.NE;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveE))
            direction = MoveDirection.E;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveSE))
            direction = MoveDirection.SE;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveS))
            direction = MoveDirection.S;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveSW))
            direction = MoveDirection.SW;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveW))
            direction = MoveDirection.W;
        else if (InputBinder.PerformRequestedAction(RequestedAction.MoveNW))
            direction = MoveDirection.NW;

        return direction;
    }

    public static MoveDirection GetRandomMoveDirection()
    {
        int randomDirection = RecRandom.Instance.GetRandomValue(0, Enum.GetNames(typeof(MoveDirection)).Length - 1);
        return (MoveDirection)randomDirection;
    }
}
