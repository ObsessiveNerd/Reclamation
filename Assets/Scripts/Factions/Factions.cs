using System;
using System.Collections.Generic;

public enum FactionId
{
    None,
    DwarvenExpedition,
    LostExpedition,
    Goblins,
    Beasts,
    Dragons,
    Demons,
    Wraiths,
    Kobolds,
    Wolves,
    Corrupted,
    Undead
}

public class Faction : Component
{
    public FactionId ID;

    public Faction(string faction)
    {
        ID = (FactionId)Enum.Parse(typeof(FactionId), faction);
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
        string[] kvp = data.Split('=');
        if(kvp.Length == 2)
        {
            if(kvp[1].Contains("<"))
            {
                string name = kvp[1].Substring(kvp[1].IndexOf('<') + 1, kvp[1].IndexOf('>') - (kvp[1].IndexOf('<') + 1));
                Component = new Faction(name);
            }
            else
            {
                Component = new Faction(kvp[1]);
            }
        }
        else
        {
            string name = data.Substring(data.IndexOf('<') + 1, data.IndexOf('>') - (data.IndexOf('<') + 1));
            Component = new Faction(name);
        }
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
    Neutral = 1,
    Friendly = 2,
    Hostile = 3,
}

public static class Factions
{
    static int[,] FactionRelation = new int[,]
    {
                                //None  //DE     LE    Gob    BE    Drag  Dem    WR   Kob    WF   CRPT   UD    
        /*None*/                { 1,     3,       3,    1,    3,    3,    3,    3,    3,    3,    3,    3},
        /*DwarvenExpedition*/   { 3,     2,       2,    3,    3,    3,    3,    3,    3,    3,    3,    3},
        /*LostExpedition*/      { 3,     2,       2,    3,    3,    3,    3,    3,    3,    3,    3,    3},
        /*Goblins*/             { 3,     3,       3,    2,    2,    1,    1,    1,    1,    1,    1,    1},
        /*Beasts*/              { 3,     3,       3,    2,    2,    1,    1,    1,    1,    1,    1,    1},
        /*Dragons*/             { 3,     3,       3,    1,    1,    1,    1,    1,    1,    1,    1,    1},
        /*Demons*/              { 3,     3,       3,    1,    1,    1,    2,    1,    1,    1,    1,    1},
        /*Wraiths*/             { 3,     3,       3,    1,    1,    1,    1,    2,    1,    1,    1,    1},
        /*Kobolds*/             { 3,     3,       3,    1,    1,    1,    1,    1,    2,    1,    1,    1},
        /*Wolves*/              { 3,     3,       3,    1,    1,    1,    1,    1,    1,    2,    1,    1},
        /*Corrupted*/           { 3,     3,       3,    1,    1,    1,    1,    1,    1,    1,    2,    2},
        /*Undead*/              { 3,     3,       3,    1,    1,    1,    1,    1,    1,    1,    2,    2}
    };

    public static Demeanor GetDemeanorForTarget(IEntity source, IEntity target)
    {
        GameEvent getSourceFaction = GameEventPool.Get(GameEventId.GetFaction).With(EventParameters.Value, null);
        Faction sourceFaction = (Faction)source.FireEvent(source, getSourceFaction).Paramters[EventParameters.Value];
        getSourceFaction.Release();

        GameEvent getTargetFaction = GameEventPool.Get(GameEventId.GetFaction).With(EventParameters.Value, null);
        Faction targetFaction = (Faction)target.FireEvent(target, getTargetFaction).Paramters[EventParameters.Value];
        getTargetFaction.Release();

        if (sourceFaction == null || targetFaction == null)
            return Demeanor.None;

        return (Demeanor)FactionRelation[(int)sourceFaction.ID, (int)targetFaction.ID];
    }
}
