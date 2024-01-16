using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtility
{
    public static MoveDirection GetMoveDirection()
    {
        MoveDirection direction = MoveDirection.None;

        bool moveNorth = GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveN);
        bool moveSouth = GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveS);
        bool moveEast = GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveE);
        bool moveWest = GameKeyInputBinder.PerformConsistentAction(RequestedAction.MoveW);

        if (moveNorth)
        {
            direction = MoveDirection.N;
            if (moveEast)
                direction = MoveDirection.NE;
            else if (moveWest)
                direction = MoveDirection.NW;
        }

        else if (moveSouth)
        {
            direction = MoveDirection.S;
            if (moveEast)
                direction = MoveDirection.SE;
            else if (moveWest)
                direction = MoveDirection.SW;
        }

        else if (moveEast)
            direction = MoveDirection.E;
        else if (moveWest)
            direction = MoveDirection.W;

        return direction;
    }

    public static MoveDirection GetRandomMoveDirection()
    {
        int randomDirection = RecRandom.Instance.GetRandomValue(0, Enum.GetNames(typeof(MoveDirection)).Length - 1);
        return (MoveDirection)randomDirection;
    }
}
