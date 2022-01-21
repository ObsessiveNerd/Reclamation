using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaOfEffectType
{
    None = 0,
    Circle,
    Cone
}

public class AreaOfEffect : Component
{
    public int Range;
    public AreaOfEffectType AOEType;

    List<Point> m_VisibleTiles = new List<Point>();

    public AreaOfEffect(int range, AreaOfEffectType aoeType)
    {
        Range = range;
        AOEType = aoeType;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AffectArea);
        RegisteredEvents.Add(GameEventId.SelectTile);
        RegisteredEvents.Add(GameEventId.SelectNewTileInDirection);
        RegisteredEvents.Add(GameEventId.EndSelection);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AffectArea)
        {
            Action<IEntity> effect = gameEvent.GetValue<Action<IEntity>>(EventParameters.Effect);

            foreach(var visibleTile in m_VisibleTiles)
                effect.Invoke(WorldUtility.GetEntityAtPosition(visibleTile));

            GameEvent endSelection = GameEventPool.Get(GameEventId.EndSelection)
                .With(EventParameters.TilePosition, null);

            foreach (var tile in m_VisibleTiles)
                Services.TileSelectionService.EndTileSelection(tile);
            
            m_VisibleTiles.Clear();
            endSelection.Release();
        }

        else if(gameEvent.ID == GameEventId.SelectTile)
        {
            bool sendWorldEvents = true;
            if(gameEvent.HasParameter(EventParameters.Value))
                sendWorldEvents = gameEvent.GetValue<bool>(EventParameters.Value);
            SelectAroundPosition(gameEvent, sendWorldEvents);
        }

        else if(gameEvent.ID == GameEventId.SelectNewTileInDirection)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
            foreach(var tile in m_VisibleTiles)
            {
                Services.TileSelectionService.EndTileSelection(tile);
            }
            GameEvent builder = GameEventPool.Get(GameEventId.SelectTile)
                                    .With(EventParameters.TilePosition, p);
            SelectAroundPosition(builder, true);
            builder.Release();
        }

        else if(gameEvent.ID == GameEventId.EndSelection)
        {
            GameEvent endSelection = GameEventPool.Get(GameEventId.EndSelection)
                .With(EventParameters.TilePosition, null);

            foreach (var tile in m_VisibleTiles)
                Services.TileSelectionService.EndTileSelection(tile);
            
            m_VisibleTiles.Clear();
            endSelection.Release();
        }
    }

    void SelectAroundPosition(GameEvent gameEvent, bool sendWorldEvents)
    {
        Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
        Shadowcasting sc = new Shadowcasting();
        var visibleTiles = sc.GetVisibleTiles(WorldUtility.GetEntityAtPosition(p), Range);
        m_VisibleTiles = visibleTiles;
        m_VisibleTiles.Add(p);

        if (sendWorldEvents)
        {
            foreach (var point in visibleTiles)
                Services.TileSelectionService.SelectTile(point);
        }
    }
}

public class DTO_AreaOfEffect : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var kvp = data.Split(',');
        int range = 0;
        AreaOfEffectType aoeType = AreaOfEffectType.None;

        foreach(var s in kvp)
        {
            string key = s.Split('=')[0];
            string value = s.Split('=')[1];

            if (key == "Range")
                range = int.Parse(value);
            if (key == "AOEType")
                aoeType = (AreaOfEffectType)Enum.Parse(typeof(AreaOfEffectType), value);
        }
        Component = new AreaOfEffect(range, aoeType);
    }

    public string CreateSerializableData(IComponent component)
    {
        AreaOfEffect aoe = (AreaOfEffect)component;
        return $"{nameof(AreaOfEffect)}: {nameof(aoe.Range)}={aoe.Range}, {nameof(aoe.AOEType)}={aoe.AOEType}";
    }
}
