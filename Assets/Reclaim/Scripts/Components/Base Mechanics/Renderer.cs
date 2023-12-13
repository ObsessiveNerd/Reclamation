using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Renderer : EntityComponent
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
            GameEvent checkForAlteredSprite = FireEvent(Self, GameEventPool.Get(GameEventId.AlterSprite)
                    .With(EventParameter.Renderer, Image)
                    .With(EventParameter.RenderSprite, gameEvent.Paramters[EventParameter.RenderSprite]));

            Point pos = Self.GetComponent<Position>().PositionPoint;
            GameObject target = Services.WorldDataQuery.GetEntityOnTile(pos);

            target.FireEvent(checkForAlteredSprite);

            Image.sprite = (Sprite)checkForAlteredSprite.Paramters[EventParameter.RenderSprite];
            checkForAlteredSprite.Release();
        }
    }
}
