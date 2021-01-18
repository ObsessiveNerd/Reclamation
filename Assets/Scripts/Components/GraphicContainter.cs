using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicContainer : Component
{
    private Sprite m_Sprite;
    public string SpritePath;

    public GraphicContainer(string spritePath)
    {
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        m_Sprite = sprite;
        SpritePath = spritePath;
        RegisteredEvents.Add(GameEventId.GetSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.RenderSprite] = m_Sprite;
    }
}

public class DTO_GraphicContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new GraphicContainer(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        GraphicContainer gc = (GraphicContainer)component;
        return $"{nameof(GraphicContainer)}:{gc.SpritePath}";
    }
}