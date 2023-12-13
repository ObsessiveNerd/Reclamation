using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AreaOfEffectType
{
    None = 0,
    Circle,
    Cone,
    Path
}

public class AreaOfEffect : EntityComponent
{
    public int Range;
    public AreaOfEffectType AOEType;
    public int RandomAffectCount = 0;

    MoveDirection savedMoveDir = MoveDirection.None;
    List<Point> m_VisibleTiles = new List<Point>();

    public AreaOfEffect(int range, AreaOfEffectType aoeType, int randomEffectCount)
    {
        Range = range;
        AOEType = aoeType;
        RandomAffectCount = randomEffectCount;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.AffectArea);
        RegisteredEvents.Add(GameEventId.SelectTile);
        RegisteredEvents.Add(GameEventId.SelectNewTileInDirection);
        RegisteredEvents.Add(GameEventId.EndSelection);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AffectArea)
        {
            Action<GameObject> effect = gameEvent.GetValue<Action<GameObject>>(EventParameter.Effect);

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
                .With(EventParameter.TilePosition, null);

            foreach (var tile in m_VisibleTiles)
                Services.TileSelectionService.EndTileSelection(tile);
            
            m_VisibleTiles.Clear();
            endSelection.Release();
        }

        else if(gameEvent.ID == GameEventId.SelectTile)
        {
            GameObject source = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Source));
            MoveDirection dir = gameEvent.GetValue<MoveDirection>(EventParameter.InputDirection);
            SelectAroundPosition(gameEvent, source, dir);
        }

        else if(gameEvent.ID == GameEventId.SelectNewTileInDirection)
        {
            Point p = gameEvent.GetValue<Point>(EventParameter.TilePosition);
            MoveDirection dir = gameEvent.GetValue<MoveDirection>(EventParameter.InputDirection);
            GameObject source = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Source));
            GameEvent builder = GameEventPool.Get(GameEventId.SelectTile)
                                    .With(EventParameter.TilePosition, p);
            SelectAroundPosition(builder, source, dir);
            builder.Release();
        }

        else if(gameEvent.ID == GameEventId.EndSelection)
        {
            GameEvent endSelection = GameEventPool.Get(GameEventId.EndSelection)
                .With(EventParameter.TilePosition, null);

            foreach (var tile in m_VisibleTiles)
                Services.TileSelectionService.EndTileSelection(tile);
            
            m_VisibleTiles.Clear();
            endSelection.Release();
        }
    }

    void SelectAroundPosition(GameEvent gameEvent, GameObject source, MoveDirection direction = MoveDirection.None)
    {
        foreach (var tile in m_VisibleTiles)
        {
            Services.TileSelectionService.EndTileSelection(tile);
        }
        m_VisibleTiles.Clear();

        Point p = gameEvent.GetValue<Point>(EventParameter.TilePosition);
        List<Point> visibleTiles = new List<Point>();
        if (AOEType == AreaOfEffectType.Circle)
        {
            Shadowcasting sc = new Shadowcasting();
            visibleTiles = sc.GetVisibleTiles(Services.WorldDataQuery.GetEntityOnTile(p), Range);
            m_VisibleTiles.Add(p);
        }
        else if (AOEType == AreaOfEffectType.Path)
        {
            AStar aStar = new AStar(200);
            visibleTiles = aStar.CalculatePath(Services.WorldDataQuery.GetPointWhereEntityIs(source), p);
            m_VisibleTiles.Add(p);
        }
        else if(AOEType == AreaOfEffectType.Cone)
        {
            int octant = ConvertDirectionToOctant(direction);
            Shadowcasting sc = new Shadowcasting();
            visibleTiles = sc.GetVisibleTiles(source, Range, new List<int>() { octant });
            visibleTiles.Remove(Services.EntityMapService.GetPointWhereEntityIs(source));
            Services.TileSelectionService.EndTileSelection(p);
            savedMoveDir = MoveDirection.None;
        }

        m_VisibleTiles.AddRange(visibleTiles);

        foreach (var point in m_VisibleTiles)
                Services.TileSelectionService.SelectTile(point);
    }

    int ConvertDirectionToOctant(MoveDirection dir)
    {
        int octant = 1;
        switch (dir)
        {
            case MoveDirection.N:
                octant = 6;
                break;
            case MoveDirection.NE:
                octant = 5;
                break;
            case MoveDirection.E:
                octant = 4;
                break;
            case MoveDirection.SE:
                octant = 3;
                break;
            case MoveDirection.S:
                octant = 2;
                break;
            case MoveDirection.SW:
                octant = 1;
                break;
            case MoveDirection.W:
                octant = 8;
                break;
            case MoveDirection.NW:
                octant = 7;
                break;
        }
        return octant;
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
            if (key == "RandomAffectCount" && !string.IsNullOrEmpty(value))
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
