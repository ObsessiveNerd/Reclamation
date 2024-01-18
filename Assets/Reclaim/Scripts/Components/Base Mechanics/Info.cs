using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class InfoData : EntityComponent
{
    public string InfoMessage;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.ShowInfo, ShowInfo);
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"CustomText", InfoMessage);
    }

    void ShowInfo(GameEvent gameEvent)
    {
        gameEvent.GetValue<StringBuilder>(EventParameter.Info).AppendLine(InfoMessage);
        RecLog.Log(InfoMessage);
    }
}

public class Info : EntityComponentBehavior
{
    InfoData Data = new InfoData();

    public override IComponent GetData()
    {
        return Data;
    }
}