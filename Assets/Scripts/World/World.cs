using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : Component
{
    private static World m_Instance;
    public static World Instance { get { return m_Instance; } }

    GameObject m_TilePrefab;

    Dictionary<IEntity, TimeProgression> m_PlayerToTimeProgressionMap = new Dictionary<IEntity, TimeProgression>();
    Dictionary<Point, Actor> m_Tiles = new Dictionary<Point, Actor>();
    Dictionary<IEntity, Point> m_EntityToPointMap = new Dictionary<IEntity, Point>();
    LinkedList<IEntity> m_Players = new LinkedList<IEntity>();
    LinkedListNode<IEntity> m_ActivePlayer;

    int m_Vertical, m_Horizontal, m_Columns, m_Rows;

    //Temp
    public List<Sprite> m_TempTerrain;
    public Sprite m_TempSelection;

    public World(IEntity self, GameObject tilePrefab, List<Sprite> tempTerrain, Sprite tempSelection)
    {
        if (m_Instance == null)
            m_Instance = this;
        else
            return;

        Init(self);

        m_TilePrefab = tilePrefab;
        m_TempTerrain = tempTerrain; //temp
        m_TempSelection = tempSelection;

        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
        m_Columns = m_Horizontal * 2;
        m_Rows = m_Vertical * 2;

        RegisteredEvents.Add(GameEventId.StartWorld);
        RegisteredEvents.Add(GameEventId.UpdateWorldView);
        RegisteredEvents.Add(GameEventId.Spawn);
        RegisteredEvents.Add(GameEventId.Despawn);
        RegisteredEvents.Add(GameEventId.MoveEntity);
        RegisteredEvents.Add(GameEventId.Interact);
        RegisteredEvents.Add(GameEventId.Drop);
        RegisteredEvents.Add(GameEventId.RotateActiveCharacter);
        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.SelectTile);
        RegisteredEvents.Add(GameEventId.SelectNewTileInDirection);
        RegisteredEvents.Add(GameEventId.EndSelection);
        RegisteredEvents.Add(GameEventId.GetActivePlayer);
        RegisteredEvents.Add(GameEventId.ShowTileInfo);
    }

    public void RegisterPlayer(IEntity entity)
    {
        m_Players.AddLast(entity);
        if (m_ActivePlayer == null)
            m_ActivePlayer = m_Players.First;
        m_PlayerToTimeProgressionMap[entity] = new TimeProgression();
        m_PlayerToTimeProgressionMap[entity].RegisterEntity(entity);
    }

    public void UnregisterPlayer(IEntity entity)
    {
        //May need to rotate to the next active character first

        m_Players.Remove(entity);
        m_PlayerToTimeProgressionMap.Remove(entity);
        FireEvent(Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                            new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Creature)));
    }

    public void ProgressTime()
    {
        m_PlayerToTimeProgressionMap[m_ActivePlayer.Value].Update();
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.StartWorld)
        {
            for (int i = 0; i < m_Columns; i++)
            {
                for (int j = 0; j < m_Rows; j++)
                {
                    CreateTile(i, j);
                }
            }
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
        }

        if (gameEvent.ID == GameEventId.UpdateWorldView)
        {
            foreach (var tile in m_Tiles)
                tile.Value.FireEvent(tile.Value, new GameEvent(GameEventId.UpdateTile));
        }

        if(gameEvent.ID == GameEventId.Spawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            Point spawnPoint = (Point)gameEvent.Paramters[EventParameters.Point];
            FireEvent(m_Tiles[spawnPoint], gameEvent);
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
            m_EntityToPointMap[entity] = spawnPoint;

            //Todo: we'll need to make sure we find which player is closest before picking their time system
            FireEvent(entity, new GameEvent(GameEventId.RegisterWithTimeSystem, new KeyValuePair<string, object>(EventParameters.Value, m_PlayerToTimeProgressionMap[m_ActivePlayer.Value])));
        }

        if(gameEvent.ID == GameEventId.Despawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            Point currentPoint = m_EntityToPointMap[entity];
            GameEvent despawn = new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                   new KeyValuePair<string, object>(EventParameters.EntityType, entityType));
            FireEvent(m_Tiles[currentPoint], despawn);
            m_EntityToPointMap.Remove(entity);
        }

        if(gameEvent.ID == GameEventId.MoveEntity)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            Point currentPoint = m_EntityToPointMap[entity];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];
            Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);

            if (m_Tiles.ContainsKey(newPoint))
            {
                GameEvent despawn = new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                   new KeyValuePair<string, object>(EventParameters.EntityType, entityType));
                FireEvent(m_Tiles[currentPoint], despawn);

                GameEvent spawn = new GameEvent(GameEventId.Spawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                       new KeyValuePair<string, object>(EventParameters.EntityType, entityType));

                FireEvent(m_Tiles[newPoint], spawn);
                FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
                m_EntityToPointMap[entity] = newPoint;
            }
            else
            {
                //Todo move to a new section of the map
            }
        }

        if(gameEvent.ID == GameEventId.Interact)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            Point currentPoint = m_EntityToPointMap[entity];
            FireEvent(m_Tiles[currentPoint], new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, entity)));
        }

        if(gameEvent.ID == GameEventId.Drop)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            IEntity droppingEntity = (IEntity)gameEvent.Paramters[EventParameters.Creature];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];

            Point p = m_EntityToPointMap[droppingEntity];
            FireEvent(m_Tiles[p], new GameEvent(GameEventId.Spawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                    new KeyValuePair<string, object>(EventParameters.EntityType, entityType)));
        }

        if(gameEvent.ID == GameEventId.RotateActiveCharacter)
        {
            m_ActivePlayer.Value.RemoveComponent(typeof(PlayerInputController));
            m_ActivePlayer = m_ActivePlayer.Next;
            if (m_ActivePlayer == null)
                m_ActivePlayer = m_Players.First;
            m_ActivePlayer.Value.AddComponent(new PlayerInputController(m_ActivePlayer.Value));
            m_ActivePlayer.Value.CleanupComponents();
        }

        if(gameEvent.ID == GameEventId.BeforeMoving)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];
            Point currentPoint = m_EntityToPointMap[entity];
            Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);
            FireEvent(m_Tiles[newPoint], gameEvent);
        }

        if(gameEvent.ID == GameEventId.GetActivePlayer)
        {
            gameEvent.Paramters[EventParameters.Entity] = m_ActivePlayer.Value;
        }

        if(gameEvent.ID == GameEventId.SelectTile)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            IEntity target = (IEntity)gameEvent.Paramters[EventParameters.Target];

            Point p = m_EntityToPointMap[target == null ? entity : target];

            //This should probably move to the tile?
            m_Tiles[p].AddComponent(new SelectedTile(m_TempSelection));
            m_Tiles[p].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = p;
        }

        if(gameEvent.ID == GameEventId.SelectNewTileInDirection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];

            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();

            Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);

            m_Tiles[newPoint].AddComponent(new SelectedTile(m_TempSelection));
            m_Tiles[newPoint].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = newPoint;
        }

        if(gameEvent.ID == GameEventId.EndSelection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();
            gameEvent.Paramters[EventParameters.TilePosition] = null;
        }

        if(gameEvent.ID == GameEventId.ShowTileInfo)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            Debug.Log($"Showing info for tile at {currentTilePos.x}, {currentTilePos.y}");
        }
    }

    public IEntity GetClosestEnemyTo(IEntity e)
    {
        return null;
    }

    void CreateTile(int x, int y)
    {
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);
        tile.transform.position = new Vector2(x - (m_Horizontal - 0.5f), y - (m_Vertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new GraphicContainter(UnityEngine.Random.Range(0f, 100f) < 30f ? m_TempTerrain[UnityEngine.Random.Range(0, m_TempTerrain.Count)] : null));
        actor.AddComponent(new Renderer(actor, tile.GetComponent<SpriteRenderer>()));
        actor.CleanupComponents();

        m_Tiles.Add(new Point(x, y), actor);
    }

    Point GetTilePointInDirection(Point basePoint, MoveDirection direction)
    {
        if (direction == MoveDirection.None)
            return basePoint;

        int x = basePoint.x;
        int y = basePoint.y;
        string name = Enum.GetName(typeof(MoveDirection), direction);
        if (name.Contains("N"))
            y++;
        if (name.Contains("S"))
            y--;
        if (name.Contains("E"))
            x++;
        if (name.Contains("W"))
            x--;
        return new Point(x, y);
    }
}
