using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptForDirectionController : InputControllerBase
{
    public PromptForDirectionController(IEntity self)
    {
        Init(self);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            Debug.Log("Waiting for input");
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                Debug.Log($"{desiredDirection} pressed");

                //Need to fire the correct event, whatever that may be
                //FireEvent(Self, new GameEvent(GameEventId.MoveKeyPressed, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)));

                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController(Self));
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController(Self));
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
        }
    }
}
