﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visible : Component
{
    public bool IsVisible;
    public bool HasBeenVisited;

    public override int Priority => 1;

    public Visible(bool hasBeenVisited)
    {
        HasBeenVisited = hasBeenVisited;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SetVisibility);
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SetVisibility)
        {
            bool tileInsight = (bool)gameEvent.Paramters[EventParameters.TileInSight];
            IsVisible = tileInsight;
            if (!HasBeenVisited && IsVisible)
                HasBeenVisited = true;
            FireEvent(Self, new GameEvent(GameEventId.VisibilityUpdated, new KeyValuePair<string, object>(EventParameters.Value, IsVisible)));
        }

        if(gameEvent.ID == GameEventId.AlterSprite)
        {
            SpriteRenderer sr = (SpriteRenderer)gameEvent.Paramters[EventParameters.Renderer];
            if (!IsVisible && !HasBeenVisited)
                sr.color = new Color(0, 0, 0, 0);
            if (!IsVisible && HasBeenVisited)
                sr.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}

public class DTO_Visible : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        bool hasBeenVisited = false;

        string[] parameters = data.Split(',');
        foreach(string param in parameters)
        {
            string[] values = param.Split('=');
            switch(values[0])
            {
                case "HasBeenVisited":
                    hasBeenVisited = bool.Parse(values[1]);
                    break;
            }
        }

        Component = new Visible(hasBeenVisited);
    }

    public string CreateSerializableData(IComponent component)
    {
        Visible v = (Visible)component;
        return $"{nameof(Visible)}: HasBeenVisited={v.HasBeenVisited}";
    }
}
