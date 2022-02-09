using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptForDirectionController : InputControllerBase
{
    Action<MoveDirection> m_Callback;

    public PromptForDirectionController(Action<MoveDirection> callback)
    {
        m_Callback = callback;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            RecLog.Log("Waiting for input");
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                RecLog.Log($"{desiredDirection} pressed");
                m_Callback(desiredDirection);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
            }
        }
    }
}

public class DTO_PromptForDirectionController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PlayerInputController();
    }

    public string CreateSerializableData(IComponent comp)
    {
        return nameof(PlayerInputController);
    }
}