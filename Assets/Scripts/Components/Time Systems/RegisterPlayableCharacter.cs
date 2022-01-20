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
            FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)
                .With(EventParameters.Entity, Self.ID)).Release();

        if (gameEvent.ID == GameEventId.Died)
            FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.UnRegisterPlayer)
                .With(EventParameters.Entity, Self.ID)).Release();
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