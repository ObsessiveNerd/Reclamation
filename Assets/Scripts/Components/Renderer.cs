using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Renderer : Component
{
    SpriteRenderer m_Image;

    public Renderer(IEntity self, SpriteRenderer image)
    {
        Init(self);
        m_Image = image;
        RegisteredEvents.Add(GameEventId.UpdateRenderer);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        GameEvent getRenderSprite = new GameEvent(GameEventId.GetRenderSprite, new KeyValuePair<string, object>(EventParameters.RenderSprite, null));
        FireEvent(Self, getRenderSprite);
        m_Image.sprite = (Sprite)getRenderSprite.Paramters[EventParameters.RenderSprite];
    }
}
