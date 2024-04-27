using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class EC_SmoothMove : EntityComponent
{
    public float Speed;
    public Rigidbody2D Rigidbody2D;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed, MoveKeyPressed);
    }

    void MoveKeyPressed(GameEvent gameEvent)
    {
        var moveX = gameEvent.GetValue<float>(EventParameter.InputX);
        var moveY = gameEvent.GetValue<float>(EventParameter.InputY);

        Rigidbody2D.velocity = new Vector2(moveX, moveY).normalized * Speed;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class SmoothMove : ComponentBehavior<EC_SmoothMove>
{
    public float Speed;
    private void Awake()
    {
        var ec_smoothMove = GetComponent() as EC_SmoothMove;
        if(ec_smoothMove != null)
        { 
            ec_smoothMove.Rigidbody2D = GetComponent<Rigidbody2D>();
            ec_smoothMove.Speed = Speed;
        }
    }
}