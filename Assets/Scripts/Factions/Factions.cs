using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction : Component
{
    public string ID { get; internal set; }

    public Faction(IEntity self, string faction)
    {
        Init(self);
        ID = faction;
        RegisteredEvents.Add(GameEventId.GetFaction);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.Value] = this;
    }
}

public enum Demeanor
{
    Neutral,
    Friendly,
    Hostile,
}

public static class Factions
{
    public const string DwarvenCompany = nameof(DwarvenCompany);
    public const string Goblins = nameof(Goblins);

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
            {Goblins, Demeanor.Hostile }
        });

        m_DemeanorMap.Add(Goblins, new Dictionary<string, Demeanor>()
        {
            {Goblins, Demeanor.Friendly },
            {DwarvenCompany, Demeanor.Hostile }
        });
    }

    public static Demeanor GetDemeanorForTarget(IEntity source, IEntity target)
    {
        GameEvent getFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
        Faction sourceFaction = (Faction)source.FireEvent(source, getFaction).Paramters[EventParameters.Value];
        Faction targetFaction = (Faction)target.FireEvent(target, getFaction).Paramters[EventParameters.Value];

        return m_DemeanorMap[sourceFaction.ID][targetFaction.ID];
    }
}
