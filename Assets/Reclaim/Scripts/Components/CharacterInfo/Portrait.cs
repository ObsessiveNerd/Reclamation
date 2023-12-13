using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : EntityComponent
{
    Sprite m_Sprite;
    public string SpritePath;

    public Portrait(string spritePath)
    {
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        m_Sprite = sprite;
        SpritePath = spritePath;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetPortrait);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetPortrait)
        {
            if(m_Sprite == null)
                Debug.Log("Sprite null");
            gameEvent.Paramters[EventParameter.RenderSprite] = m_Sprite;
        }
    }
}

public class DTO_Portrait : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        if(data.Contains("="))
            Component = new Portrait(data.Split('=')[1]);
        else
            Component = new Portrait(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        Portrait gc = (Portrait)component;
        return $"{nameof(Portrait)}:{nameof(gc.SpritePath)}={gc.SpritePath}";
    }
}
