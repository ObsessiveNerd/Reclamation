using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Info : EntityComponent
{
    public string InfoMessage;
    public override int Priority { get { return 10; } }

    public Info(string info)
    {
        InfoMessage = info;
        RegisteredEvents.Add(GameEventId.ShowInfo);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ShowInfo)
        {
            gameEvent.GetValue<StringBuilder>(EventParameters.Info).AppendLine(InfoMessage);
            RecLog.Log(InfoMessage);
        }
        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
            dictionary.Add($"CustomText", InfoMessage);
        }
    }
}

public class DTO_Info : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if(value.Contains("="))
            value = data.Split('=')[1];
        Component = new Info(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        Info info = (Info)component;
        return $"{nameof(Info)}: {nameof(info.InfoMessage)}={info.InfoMessage}";
    }
}