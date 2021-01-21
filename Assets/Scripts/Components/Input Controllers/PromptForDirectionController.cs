using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptForDirectionController : InputControllerBase
{
    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            RecLog.Log("Waiting for input");
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                RecLog.Log($"{desiredDirection} pressed");

                FireEvent(Self, new GameEvent(GameEventId.PromptForInput, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)));

                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
        }
    }
}

public class DTO_PromptForDirectionController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PromptForDirectionController();
    }

    public string CreateSerializableData(IComponent comp)
    {
        return nameof(PromptForDirectionController);
    }
}