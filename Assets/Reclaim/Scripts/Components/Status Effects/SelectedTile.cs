﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTile : EntityComponent
{
    Sprite m_SelectionSprite;

    public override int Priority { get { return 6; } }

    public void Start()
    {
        

        m_SelectionSprite = Resources.Load<Sprite>("Textures/Effects/selection");
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.GetValue<SpriteRenderer>(EventParameter.Renderer).color = Color.blue; //= m_SelectionSprite;
    }
}
