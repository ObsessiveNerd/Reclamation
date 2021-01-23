using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : InputControllerBase
{
    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();
            if (desiredDirection != MoveDirection.None)
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.UIInput, new KeyValuePair<string, object>(EventParameters.Value, desiredDirection)));

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.CloseUI));
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
            }
        }
    }
}

public class DTO_PlayerUIController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PlayerInputController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerInputController);
    }
}