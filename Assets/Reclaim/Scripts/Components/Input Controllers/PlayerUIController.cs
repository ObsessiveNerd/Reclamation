using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : InputControllerBase
{
    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            if (UIManager.UIClear)
            {
                //FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.CloseUI));
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                RotateActiveCharacter(gameEvent);
            }
        }
    }

    void RotateActiveCharacter(GameEvent gameEvent)
    {
        Services.PlayerManagerService.RotateActiveCharacter();
        Services.WorldUpdateService.UpdateWorldView();
        gameEvent.ContinueProcessing = false;
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