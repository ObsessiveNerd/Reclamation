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

        if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveN))
            direction = MoveDirection.N;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveNE))
            direction = MoveDirection.NE;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveE))
            direction = MoveDirection.E;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveSE))
            direction = MoveDirection.SE;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveS))
            direction = MoveDirection.S;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveSW))
            direction = MoveDirection.SW;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveW))
            direction = MoveDirection.W;
        else if (KeyInputBinder.PerformRequestedAction(RequestedAction.MoveNW))
            direction = MoveDirection.NW;

        return direction;
    }

    public static MoveDirection GetRandomMoveDirection()
    {
        int randomDirection = RecRandom.Instance.GetRandomValue(0, Enum.GetNames(typeof(MoveDirection)).Length - 1);
        return (MoveDirection)randomDirection;
    }
}
