using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterPlayableCharacter : Component
{
    public override int Priority => 10;
    public RegisterPlayableCharacter()
    {
        RegisteredEvents.Add(GameEventId.RegisterPlayableCharacter);
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RegisterPlayableCharacter)
            Services.PlayerManagerService.RegisterPlayer(Self);

        if (gameEvent.ID == GameEventId.Died)
            Services.PlayerManagerService.UnRegisterPlayer(Self);
    }
}

public class DTO_RegisterPlayableCharacter : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new RegisterPlayableCharacter();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(RegisterPlayableCharacter);
    }
}