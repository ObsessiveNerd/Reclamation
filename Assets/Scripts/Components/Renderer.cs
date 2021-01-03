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
        m_Image.color = Color.white;
        GameEvent checkForAlteredSprite = FireEvent(Self, new GameEvent(GameEventId.AlterSprite, new KeyValuePair<string, object>(EventParameters.Renderer, m_Image),
                                                                new KeyValuePair<string, object>(EventParameters.RenderSprite, gameEvent.Paramters[EventParameters.RenderSprite])));
        m_Image.sprite = (Sprite)checkForAlteredSprite.Paramters[EventParameters.RenderSprite];
    }
}
