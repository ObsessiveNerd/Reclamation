using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpellContainer : Component
{
    //private List<string> m_SpellIds = new List<string>();
    public Dictionary<string, IEntity> SpellNameToIdMap = new Dictionary<string, IEntity>();

    public SpellContainer(string spellNames)
    {
        foreach(var spell in EntityFactory.GetEntitiesFromArray(spellNames))
            SpellNameToIdMap[spell.Name] = spell;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetSpells);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetSpells)
        {
            foreach (var key in SpellNameToIdMap.Keys)
                gameEvent.GetValue<List<string>>(EventParameters.SpellList).Add(SpellNameToIdMap[key].ID);
        }
    }
}

public class DTO_SpellContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new SpellContainer(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        SpellContainer sc = (SpellContainer)component;
        StringBuilder spellNameBuilder = new StringBuilder();
        foreach (var name in sc.SpellNameToIdMap.Keys)
            spellNameBuilder.Append($"<{name}>,");
        return $"{nameof(SpellContainer)}: [{spellNameBuilder.ToString().TrimEnd(',')}]";
    }
}
