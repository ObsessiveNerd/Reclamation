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

        if (Input.GetKeyDown(KeyCode.Keypad8))
            direction = MoveDirection.N;
        else if (Input.GetKeyDown(KeyCode.Keypad9))
            direction = MoveDirection.NE;
        else if (Input.GetKeyDown(KeyCode.Keypad6))
            direction = MoveDirection.E;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            direction = MoveDirection.SE;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            direction = MoveDirection.S;
        else if (Input.GetKeyDown(KeyCode.Keypad1))
            direction = MoveDirection.SW;
        else if (Input.GetKeyDown(KeyCode.Keypad4))
            direction = MoveDirection.W;
        else if (Input.GetKeyDown(KeyCode.Keypad7))
            direction = MoveDirection.NW;

        return direction;
    }

    public static MoveDirection GetRandomMoveDirection()
    {
        int randomDirection = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MoveDirection)).Length - 1);
        return (MoveDirection)randomDirection;
    }
}
