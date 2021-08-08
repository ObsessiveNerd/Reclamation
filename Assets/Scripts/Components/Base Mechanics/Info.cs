using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : Component
{
    public string InfoMessage;
    public override int Priority { get { return 6; } }

    public Info(string info)
    {
        InfoMessage = info;
        RegisteredEvents.Add(GameEventId.ShowInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        RecLog.Log(InfoMessage);
    }
}

public class DTO_Info : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data.Split('=')[1];
        Component = new Info(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        Info info = (Info)component;
        return $"{nameof(Info)}: {info.InfoMessage}";
    }
}