using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterPlayableCharacter : Component
{
    public RegisterPlayableCharacter()
    {
        RegisteredEvents.Add(GameEventId.RegisterPlayableCharacter);
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RegisterPlayableCharacter)
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.RegisterPlayableCharacter, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));

        if (gameEvent.ID == GameEventId.Died)
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.UnRegisterPlayer, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
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