using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetSpells);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
            gameEvent.GetValue<List<string>>(EventParameters.SpellList).Add(Self.ID);
    }
}

public class DTO_Spell : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Spell();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Spell);
    }
}
