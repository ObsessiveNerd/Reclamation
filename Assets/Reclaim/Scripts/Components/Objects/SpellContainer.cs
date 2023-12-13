using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpellContainer : EntityComponent
{
    public int MaxSpell = 5;
    public bool RandomSpells = true;
    public string SpecificSpell;

    Dictionary<string, GameObject> m_SpellNameToIdMap = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> SpellNameToIdMap
    {
        get
        {
            if (RandomSpells && m_SpellNameToIdMap.Count == 0)
            {
                int spellAmount = RecRandom.Instance.GetRandomValue(1, MaxSpell);
                for (int i = 0; i < spellAmount; i++)
                {
                    string bp = EntityFactory.GetRandomSpellBPName(0);
                    if (!m_SpellNameToIdMap.ContainsKey(bp))
                        m_SpellNameToIdMap.Add(bp, EntityQuery.GetEntity(bp));
                }
            }
            else if(!RandomSpells)
            {
                if(SpecificSpell.Contains("&"))
                {
                    var list = EntityFactory.GetEntitiesFromArray(SpecificSpell);
                    foreach (var e in list)
                    {
                        if (!m_SpellNameToIdMap.ContainsKey(e.InternalName))
                            m_SpellNameToIdMap.Add(e.InternalName, e);
                    }
                }
                else
                {
                    if (!m_SpellNameToIdMap.ContainsKey(SpecificSpell))
                        m_SpellNameToIdMap.Add(SpecificSpell, EntityQuery.GetEntity(SpecificSpell));
                }
            }
            return m_SpellNameToIdMap;
        }
    }

    public SpellContainer(string specSpec, int maxSpell)
    {
        MaxSpell = maxSpell;
        var spellArray = EntityFactory.GetEntitiesFromArray(specSpec);
        foreach(var spell in spellArray)
            m_SpellNameToIdMap.Add(spell.InternalName, spell);
        SpecificSpell = specSpec;
        if (!string.IsNullOrEmpty(specSpec))
            RandomSpells = false;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.GetInfo);
        RegisteredEvents.Add(GameEventId.ItemEquipped);
        RegisteredEvents.Add(GameEventId.ItemUnequipped);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
        {
            foreach (var key in SpellNameToIdMap.Keys)
                gameEvent.GetValue<HashSet<string>>(EventParameter.SpellList).Add(SpellNameToIdMap[key].ID);
        }

        else if(gameEvent.ID == GameEventId.ItemUnequipped)
        {
            foreach (var key in SpellNameToIdMap.Keys)
            {
                GameEvent removeAbility = GameEventPool.Get(GameEventId.RemoveFromActiveAbilities)
                                            .With(EventParameter.Abilities, SpellNameToIdMap.Values.ToList());

                Services.PlayerManagerService.GetActivePlayer()?.FireEvent(removeAbility).Release();
            }
        }

        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            Dictionary<string, string> info = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            info.Add($"{nameof(SpellContainer)}{Guid.NewGuid()}", "An object that contains arcane magics.  Use carefully.  Or don't.  I'm not your dad.");
        }

        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            string sourceId = gameEvent.GetValue<string>(EventParameter.Owner);
            if (WorldUtility.IsActivePlayer(sourceId))
            {
                Services.WorldUIService.OpenSpellUI();
            }
        }

        else if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            ContextMenuButton button = new ContextMenuButton("Show Spells", () =>
            {
                Services.WorldUIService.OpenSpellExaminationUI(SpellNameToIdMap.Keys.ToList());
            });

            gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(button);
        }
    }
}

public class DTO_SpellContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string dataToPass = "";
        string specSpell = "";
        int maxSpell = 5;
        if (!string.IsNullOrEmpty(data))
        {
            string[] kvp = data.Split(',');
            foreach (var dataPair in kvp)
            {
                string key = dataPair.Split('=')[0];
                string value = dataPair.Split('=')[1];

                //if (key == "SpellNameToIdMap")
                //    dataToPass = value;
                if (key == "SpecificSpell")
                    specSpell = value;
                if (key == "MaxSpell")
                    maxSpell = int.Parse(value);
            }
        }

        Component = new SpellContainer(specSpell, maxSpell);
    }

    public string CreateSerializableData(IComponent component)
    {
        SpellContainer sc = (SpellContainer)component;
        if (sc.SpellNameToIdMap.Count > 1)
        {
            StringBuilder spellNameBuilder = new StringBuilder();
            spellNameBuilder.Append($"[");
            foreach (var name in sc.SpellNameToIdMap.Keys)
                spellNameBuilder.Append($"<{name}>&");
            var totalString = spellNameBuilder.ToString().TrimEnd('&');
            totalString += "]";
            return $"{nameof(SpellContainer)}: {nameof(sc.SpecificSpell)}={totalString}, {nameof(sc.RandomSpells)}={sc.RandomSpells}, {nameof(sc.MaxSpell)}={sc.MaxSpell}";
        }
        else
            return $"{nameof(SpellContainer)}: {nameof(sc.SpecificSpell)}={sc.SpecificSpell}, {nameof(sc.RandomSpells)}={sc.RandomSpells}, {nameof(sc.MaxSpell)}={sc.MaxSpell}";
        //return $"{nameof(SpellContainer)}";
    }
}
