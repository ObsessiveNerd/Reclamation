using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Component
{
    public bool Closed;
    public bool Locked;
    public string OpenSpritePath;
    public string ClosedSpritePath;

    private Sprite m_ClosedSprite;
    private Sprite m_OpenSprite;

    public Door(bool closed, bool locked, string openSpritePath, string closedSpritePath)
    {
        Closed = closed;
        Locked = locked;
        OpenSpritePath = openSpritePath;
        ClosedSpritePath = closedSpritePath;

        m_OpenSprite = Resources.Load<Sprite>(OpenSpritePath);
        m_ClosedSprite = Resources.Load<Sprite>(ClosedSpritePath);
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Interact);
        RegisteredEvents.Add(GameEventId.PathfindingData);
        RegisteredEvents.Add(GameEventId.IsTileBlocking);
        RegisteredEvents.Add(GameEventId.GetSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Interact)
        {
            if (Locked)
            {
                //Look for key in inventory
            }
            else
                Closed = !Closed;
        }
        else if(gameEvent.ID == GameEventId.PathfindingData)
        {
            if(Locked && Closed)
            {
                gameEvent.Paramters[EventParameters.BlocksMovement] = true;
                gameEvent.Paramters[EventParameters.Weight] = Pathfinder.ImpassableWeight;
            }
        }
        else if (gameEvent.ID == GameEventId.IsTileBlocking)
        {
            if(Closed)
                gameEvent.Paramters[EventParameters.Value] = true;
        }
        else if (gameEvent.ID == GameEventId.GetSprite)
        {
            if(Closed)
                gameEvent.Paramters[EventParameters.RenderSprite] = m_ClosedSprite;
            else
                gameEvent.Paramters[EventParameters.RenderSprite] = m_OpenSprite;
        }
    }
}

public class DTO_Door : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        bool closed = true;
        bool locked = false;
        string closedSpritePath = "";
        string openSpritePath = "";

        string[] parameters = data.Split(',');
        foreach(string parameter in parameters)
        {
            string[] keyValue = parameter.Split('=');
            switch (keyValue[0])
            {
                case "Closed":
                    closed = bool.Parse(keyValue[1]);
                    break;
                case "Locked":
                    locked = bool.Parse(keyValue[1]);
                    break;
                case "ClosedSpritePath":
                    closedSpritePath = keyValue[1];
                    break;
                case "OpenSpritePath":
                    openSpritePath = keyValue[1];
                    break;
            }
        }
        Component = new Door(closed, locked, openSpritePath, closedSpritePath);
    }

    public string CreateSerializableData(IComponent component)
    {
        Door door = (Door)component;
        return $"{nameof(Door)}: Closed={door.Closed}, Locked={door.Locked}, ClosedSpritePath={door.ClosedSpritePath}, OpenSpritePath={door.OpenSpritePath}";
    }
}