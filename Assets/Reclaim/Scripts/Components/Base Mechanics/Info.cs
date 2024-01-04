using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Info : EntityComponent
{
    public string InfoMessage;
    public Info(string info)
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