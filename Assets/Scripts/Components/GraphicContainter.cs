using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicContainer : Component
{
    private Sprite m_Sprite;

    public GraphicContainer(Sprite sprite)
    {
        m_Sprite = sprite;
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
        Sprite sprite = Resources.Load<Sprite>(data);
        Component = new GraphicContainer(sprite);
    }
}