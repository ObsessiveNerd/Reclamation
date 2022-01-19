using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Renderer : Component
{
    public SpriteRenderer Image;

    public Renderer(SpriteRenderer image)
    {
        Image = image;
        RegisteredEvents.Add(GameEventId.UpdateRenderer);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateRenderer)
        {
            Image.color = Color.white;
            GameEvent checkForAlteredSprite = FireEvent(Self, new GameEvent(GameEventId.AlterSprite, new KeyValuePair<string, object>(EventParameters.Renderer, Image),
                                                                    new KeyValuePair<string, object>(EventParameters.RenderSprite, gameEvent.Paramters[EventParameters.RenderSprite])));
            Image.sprite = (Sprite)checkForAlteredSprite.Paramters[EventParameters.RenderSprite];
        }
    }
}
