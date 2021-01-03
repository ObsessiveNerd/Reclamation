using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTile : Component
{
    Sprite m_SelectionSprite;

    public override int Priority { get { return 6; } }

    public SelectedTile(Sprite selection)
    {
        m_SelectionSprite = selection;
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.RenderSprite] = m_SelectionSprite;
    }
}
