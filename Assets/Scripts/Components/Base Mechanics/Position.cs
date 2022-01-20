﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : Component
{
    public Point PositionPoint;

    public Position() { }

    public Position(Point p)
    {
        PositionPoint = p;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SetPoint);
        RegisteredEvents.Add(GameEventId.GetPoint);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.SetPoint)
            PositionPoint = (Point)gameEvent.Paramters[EventParameters.TilePosition];

        if (gameEvent.ID == GameEventId.GetPoint)
            gameEvent.Paramters[EventParameters.Value] = PositionPoint;
    }
}

public class DTO_Position : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] kvp = data.Split('=');
        if(kvp.Length == 2)
        {
            string[] parameters = kvp[1].Split(',');
            int x = 0;
            int y = 0;

            if (parameters.Length == 2)
            {
                x = int.Parse(parameters[0]);
                y = int.Parse(parameters[1]);
            }

            Point point = new Point(x, y);
            Component = new Position(point);
        }
        else if (!string.IsNullOrEmpty(data))
        {
            string[] parameters = data.Split(',');
            int x = int.Parse(parameters[0]);
            int y = int.Parse(parameters[1]);
            Point point = new Point(x, y);
            Component = new Position(point);
        }
        else
            Component = new Position();
    }

    public string CreateSerializableData(IComponent component)
    {
        var getLocation = component.FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.GetEntityLocation)
                .With(EventParameters.Entity, component.Self.ID)
                .With(EventParameters.TilePosition, null));

        if (getLocation.Paramters[EventParameters.TilePosition] != null)
        {
            Point p = (Point)getLocation.Paramters[EventParameters.TilePosition];
            getLocation.Release();
            return $"{nameof(Position)}:{p.x},{p.y}";
        }
        getLocation.Release();
        return $"{nameof(Position)}:{0},{0}";
    }
}
