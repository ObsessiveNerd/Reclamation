using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryStatType : Component
{
    public Stat PrimaryStat;

    public PrimaryStatType(Stat primaryStat)
    {
        PrimaryStat = primaryStat;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetPrimaryStatType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetPrimaryStatType)
        {
            gameEvent.Paramters[EventParameters.Value] = PrimaryStat;
        }
    }
}

public class DTO_PrimaryStatType : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] kvp = data.Split('=');
        if(kvp.Length == 2)
        {
            Stat s = (Stat)Enum.Parse(typeof(Stat), kvp[1]);
            Component = new PrimaryStatType(s);
        }
        else
        {
            Stat s = (Stat)Enum.Parse(typeof(Stat), data);
            Component = new PrimaryStatType(s);
        }
    }

    public string CreateSerializableData(IComponent component)
    {
        PrimaryStatType pst = (PrimaryStatType)component;
        return $"{nameof(PrimaryStatType)}:{pst.PrimaryStat}";
    }
}
