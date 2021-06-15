using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisible : Component
{
    public bool IsVisible;
    public bool HasBeenVisited;

    public override int Priority => 1;

    public TileVisible(bool hasBeenVisited)
    {
        HasBeenVisited = hasBeenVisited;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SetVisibility);
        RegisteredEvents.Add(GameEventId.SetHasBeenVisited);
        RegisteredEvents.Add(GameEventId.AlterSprite);
        RegisteredEvents.Add(GameEventId.GetVisibilityData);
        FireEvent(Self, new GameEvent(GameEventId.VisibilityUpdated, new KeyValuePair<string, object>(EventParameters.Value, HasBeenVisited)));
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

        if (gameEvent.ID == GameEventId.SetHasBeenVisited)
        {
            HasBeenVisited = gameEvent.GetValue<bool>(EventParameters.HasBeenVisited);
        }

        if(gameEvent.ID == GameEventId.AlterSprite)
        {
            SpriteRenderer sr = (SpriteRenderer)gameEvent.Paramters[EventParameters.Renderer];
            if (!IsVisible && !HasBeenVisited)
                sr.color = new Color(0, 0, 0, 0);
            else if (!IsVisible && HasBeenVisited)
                sr.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            else
                sr.color = Color.white;
        }

        if(gameEvent.ID == GameEventId.GetVisibilityData)
        {
            gameEvent.Paramters[EventParameters.HasBeenVisited] = HasBeenVisited;
        }
    }
}

public class DTO_TileVisible : IDataTransferComponent
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

        Component = new TileVisible(hasBeenVisited);
    }

    public string CreateSerializableData(IComponent component)
    {
        TileVisible v = (TileVisible)component;
        return $"{nameof(TileVisible)}: HasBeenVisited={v.HasBeenVisited}";
    }
}
