using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicContainter : Component
{
    private Sprite m_Sprite;

    public GraphicContainter(Sprite sprite)
    {
        m_Sprite = sprite;
        RegisteredEvents.Add(GameEventId.GetSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.RenderSprite] = m_Sprite;
    }
}
