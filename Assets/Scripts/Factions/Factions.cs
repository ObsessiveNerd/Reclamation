using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction : Component
{
    public string ID { get; internal set; }

    public Faction(string faction)
    {
        ID = faction;
        RegisteredEvents.Add(GameEventId.GetFaction);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.Value] = this;
    }
}

public class DTO_Faction : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string name = data.Substring(data.IndexOf('<') + 1, data.IndexOf('>') - (data.IndexOf('<') + 1));
        Component = new Faction(name);
    }

    public string CreateSerializableData(IComponent component)
    {
        Faction f = (Faction)component;
        return $"{nameof(Faction)}:<{f.ID}>";
    }
}


public enum Demeanor
{
    None = 0,
    Neutral,
    Friendly,
    Hostile,
}

public static class Factions
{
    public const string DwarvenCompany = nameof(DwarvenCompany);
    public const string Goblins = nameof(Goblins);
    public const string RedDragon = nameof(RedDragon);

    static Dictionary<string, Dictionary<string, Demeanor>> m_DemeanorMap = new Dictionary<string, Dictionary<string, Demeanor>>();

    private static bool m_IsInitialized = false;
    public static void Initialize()
    {
        if (m_IsInitialized)
            return;

        m_IsInitialized = true;
        m_DemeanorMap.Add(DwarvenCompany, new Dictionary<string, Demeanor>()
        {
            {DwarvenCompany, Demeanor.Friendly },
            {Goblins, Demeanor.Hostile },
            {RedDragon, Demeanor.Hostile },
        });

        m_DemeanorMap.Add(Goblins, new Dictionary<string, Demeanor>()
        {
            {Goblins, Demeanor.Friendly },
            {DwarvenCompany, Demeanor.Hostile },
            {RedDragon, Demeanor.Hostile }
        });

        m_DemeanorMap.Add(RedDragon, new Dictionary<string, Demeanor>()
        {
            {Goblins, Demeanor.Hostile },
            {DwarvenCompany, Demeanor.Hostile },
            {RedDragon, Demeanor.Friendly }
        });
    }

    public static Demeanor GetDemeanorForTarget(IEntity source, IEntity target)
    {
        GameEvent getSourceFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
        Faction sourceFaction = (Faction)source.FireEvent(source, getSourceFaction).Paramters[EventParameters.Value];

        GameEvent getTargetFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
        Faction targetFaction = (Faction)target.FireEvent(target, getTargetFaction).Paramters[EventParameters.Value];

        if (sourceFaction == null || targetFaction == null)
            return Demeanor.None;

        return m_DemeanorMap[sourceFaction.ID][targetFaction.ID];
    }
}
