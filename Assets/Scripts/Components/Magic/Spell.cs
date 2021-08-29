using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Component
{
    public int ManaCost;

    public Spell(int cost)
    {
        ManaCost = cost;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.ManaCost);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
            gameEvent.GetValue<HashSet<string>>(EventParameters.SpellList).Add(Self.ID);

        if (gameEvent.ID == GameEventId.ManaCost)
            gameEvent.Paramters[EventParameters.Value] = ManaCost;

        if(gameEvent.ID == GameEventId.GetInfo)
        {
            Dictionary<string, string> info = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
            info.Add($"{nameof(Spell)}{Guid.NewGuid()}", $"Mana cost: {ManaCost}");
        }
    }
}

public class DTO_Spell : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Spell(int.Parse(data.Split('=')[1]));
    }

    public string CreateSerializableData(IComponent component)
    {
        Spell s = (Spell)component;
        return $"{nameof(Spell)}: {nameof(s.ManaCost)}={s.ManaCost}";
    }
}
