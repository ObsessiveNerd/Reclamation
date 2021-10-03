using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpellContainer : Component
{
    public int MaxSpell = 5;
    Dictionary<string, IEntity> m_SpellNameToIdMap = new Dictionary<string, IEntity>();
    public Dictionary<string, IEntity> SpellNameToIdMap
    {
        get
        {
            if (m_SpellNameToIdMap.Count == 0)
            {
                for (int i = 0; i < RecRandom.Instance.GetRandomValue(1, MaxSpell); i++)
                {
                    string bp = EntityFactory.GetRandomSpellBPName(0);
                    if (!m_SpellNameToIdMap.ContainsKey(bp))
                        m_SpellNameToIdMap.Add(bp, EntityQuery.GetEntity(bp));
                }
            }
            return m_SpellNameToIdMap;
        }
    }

    public SpellContainer(string data)
    {
        var spellArray = EntityFactory.GetEntitiesFromArray(data);
        foreach(var spell in spellArray)
            m_SpellNameToIdMap.Add(spell.Name, spell);
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.GetInfo);
        RegisteredEvents.Add(GameEventId.ItemEquipped);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
        {
            foreach (var key in SpellNameToIdMap.Keys)
                gameEvent.GetValue<HashSet<string>>(EventParameters.SpellList).Add(SpellNameToIdMap[key].ID);
        }

        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            Dictionary<string, string> info = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
            foreach (var spell in SpellNameToIdMap.Values)
            {
                info.Add($"{nameof(SpellContainer)}{Guid.NewGuid()}", spell.Name);
                spell.FireEvent(gameEvent);
                info.Add($"{nameof(SpellContainer)}{Guid.NewGuid()}", "\n");
            }
        }

        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            string sourceId = gameEvent.GetValue<string>(EventParameters.Owner);
            if (WorldUtility.IsActivePlayer(sourceId))
            {
                EventBuilder openSpellUI = new EventBuilder(GameEventId.OpenSpellUI)
                                            .With(EventParameters.Entity, Self.ID)
                                            .With(EventParameters.SpellList, SpellNameToIdMap.Values.Select(s => s.ID).ToList());

                FireEvent(World.Instance.Self, openSpellUI.CreateEvent());
            }
        }

        else if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            ContextMenuButton button = new ContextMenuButton("Examine", () =>
            {
                EventBuilder openSpellExamination = new EventBuilder(GameEventId.OpenSpellExaminationUI)
                                                    .With(EventParameters.SpellList, SpellNameToIdMap.Keys.ToList());

                World.Instance.Self.FireEvent(openSpellExamination.CreateEvent());
            });

            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
        }
    }
}

public class DTO_SpellContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string dataToPass = "";
        if (!string.IsNullOrEmpty(data))
        {
            string[] kvp = data.Split('|');
            foreach (var dataPair in kvp)
            {
                string key = dataPair.Split('=')[0];
                string value = dataPair.Split('=')[1];

                if (key == "SpellNameToIdMap")
                    dataToPass = value;
            }
        }

        Component = new SpellContainer(dataToPass);
    }

    public string CreateSerializableData(IComponent component)
    {
        SpellContainer sc = (SpellContainer)component;
        StringBuilder spellNameBuilder = new StringBuilder();
        spellNameBuilder.Append($"{nameof(sc.MaxSpell)}={sc.MaxSpell}|");
        spellNameBuilder.Append($"{nameof(sc.SpellNameToIdMap)}=[");
        foreach (var name in sc.SpellNameToIdMap.Keys)
            spellNameBuilder.Append($"<{name}>&");
        var totalString = spellNameBuilder.ToString().TrimEnd('&');
        totalString += "]";
        return $"{nameof(SpellContainer)}: {totalString}";
        //return $"{nameof(SpellContainer)}";
    }
}
