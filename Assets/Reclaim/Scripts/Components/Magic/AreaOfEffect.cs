using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AreaOfEffectType
{
    None = 0,
    Circle,
    Cone
}

public class AreaOfEffect : EntityComponent
{
    public int Range;
    public AreaOfEffectType AOEType;
    public int RandomAffectCount = 0;

    List<Point> m_VisibleTiles = new List<Point>();

    public AreaOfEffect(int range, AreaOfEffectType aoeType, int randomEffectCount)
    {
        Range = range;
        AOEType = aoeType;
        RandomAffectCount = randomEffectCount;
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

            var tilesToAffect = new List<Point>();
            if (RandomAffectCount > 0 && m_VisibleTiles.Count > 0)
            {
                for (int i = 0; i < RandomAffectCount; i++)
                    tilesToAffect.Add(m_VisibleTiles[RecRandom.Instance.GetRandomValue(0, m_VisibleTiles.Count)]);

                tilesToAffect = tilesToAffect.Distinct().ToList();
            }
            else
                tilesToAffect = m_VisibleTiles;

            foreach(var visibleTile in tilesToAffect)
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
            SelectAroundPosition(gameEvent);
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
            SelectAroundPosition(builder);
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

    void SelectAroundPosition(GameEvent gameEvent)
    {
        Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
        Shadowcasting sc = new Shadowcasting();
        var visibleTiles = sc.GetVisibleTiles(WorldUtility.GetEntityAtPosition(p), Range);
        m_VisibleTiles = visibleTiles;
        m_VisibleTiles.Add(p);

        foreach (var point in visibleTiles)
                Services.TileSelectionService.SelectTile(point);
    }
}

public class DTO_AreaOfEffect : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var kvp = data.Split(',');
        int range = 0;
        int randomAffectArea = 0;

        AreaOfEffectType aoeType = AreaOfEffectType.None;

        foreach(var s in kvp)
        {
            string key = s.Split('=')[0];
            string value = s.Split('=')[1];

            if (key == "Range")
                range = int.Parse(value);
            if (key == "AOEType")
                aoeType = (AreaOfEffectType)Enum.Parse(typeof(AreaOfEffectType), value);
            if (key == "RandomAffectCount")
                randomAffectArea = int.Parse(value);
        }
        Component = new AreaOfEffect(range, aoeType, randomAffectArea);
    }

    public string CreateSerializableData(IComponent component)
    {
        AreaOfEffect aoe = (AreaOfEffect)component;
        return $"{nameof(AreaOfEffect)}: {nameof(aoe.Range)}={aoe.Range}, {nameof(aoe.AOEType)}={aoe.AOEType}, {nameof(aoe.RandomAffectCount)}={aoe.RandomAffectCount}";
    }
}
