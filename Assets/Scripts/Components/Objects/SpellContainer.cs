using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpellContainer : Component
{
    int m_MaxSpell = 5;
    Dictionary<string, IEntity> m_SpellNameToIdMap = new Dictionary<string, IEntity>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.ItemEquipped);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
        {
            if (m_SpellNameToIdMap.Count == 0)
            {
                for (int i = 0; i < RecRandom.Instance.GetRandomValue(1, m_MaxSpell); i++)
                {
                    string bp = EntityFactory.GetRandomSpellBPName(0);
                    if (!m_SpellNameToIdMap.ContainsKey(bp))
                        m_SpellNameToIdMap.Add(bp, EntityQuery.GetEntity(bp));
                }
            }

            foreach (var key in m_SpellNameToIdMap.Keys)
                gameEvent.GetValue<HashSet<string>>(EventParameters.SpellList).Add(m_SpellNameToIdMap[key].ID);
        }

        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            //EventBuilder getSpells = new EventBuilder(GameEventId.GetSpells)
            //                        .With(EventParameters.SpellList, m_Spells);

            //FireEvent(Self, getSpells.CreateEvent());

            EventBuilder openSpellUI = new EventBuilder(GameEventId.OpenSpellUI)
                                        .With(EventParameters.Entity, Self.ID)
                                        .With(EventParameters.SpellList, m_SpellNameToIdMap.Values.Select(s => s.ID).ToList());

            FireEvent(World.Instance.Self, openSpellUI.CreateEvent());
        }
    }
}

public class DTO_SpellContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        //string[] kvp = data.Split('|');
        //string dataToPass = "";
        //foreach(var dataPair in kvp)
        //{
        //    string key = dataPair.Split('=')[0];
        //    string value = dataPair.Split('=')[1];

        //    if (key == "SpellNameToIdMap")
        //        dataToPass = value;
        //}

        Component = new SpellContainer();
    }

    public string CreateSerializableData(IComponent component)
    {
        //SpellContainer sc = (SpellContainer)component;
        //StringBuilder spellNameBuilder = new StringBuilder();
        //spellNameBuilder.Append($"{nameof(sc.MaxSpell)}={sc.MaxSpell}|");
        //spellNameBuilder.Append($"{nameof(sc.SpellNameToIdMap)}=[");
        //foreach (var name in sc.SpellNameToIdMap.Keys)
        //    spellNameBuilder.Append($"<{name}>,");
        //var totalString = spellNameBuilder.ToString().TrimEnd(',');
        //totalString += "]";
        //return $"{nameof(SpellContainer)}: {totalString}";
        return $"{nameof(SpellContainer)}";
    }
}
