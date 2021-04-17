using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : Component
{
    Sprite m_Sprite;
    public string SpritePath;

    public Portrait(string spritePath)
    {
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        m_Sprite = sprite;
        SpritePath = spritePath;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetCharacterInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetCharacterInfo)
        {
            gameEvent.Paramters[EventParameters.RenderSprite] = m_Sprite;
        }
    }
}

public class DTO_Portrait : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Portrait(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        Portrait gc = (Portrait)component;
        return $"{nameof(Portrait)}:{gc.SpritePath}";
    }
}
