using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PointComparer : IEqualityComparer<Point>
{
    public bool Equals(Point x, Point y)
    {
        return x == y;
    }

    public int GetHashCode(Point obj)
    {
        return obj.GetHashCode();
    }
}

[Serializable]
public struct Point : INetworkSerializable
{
    public static readonly Point InvalidPoint = new Point(-1, -1);

    [SerializeField]
    private int m_x;
    [SerializeField]
    private int m_y;

    public int x { get { return m_x; } set { m_x = value; } }
    public int y { get { return m_y; } set { m_y = value; } }

    public Point(int _x, int _y)
    {
        m_x = _x;
        m_y = _y;
    }

    public Point(Vector3 vector3)
    {
        m_x = (int)Math.Round(vector3.x);
        m_y = (int)Math.Round(vector3.y);
    }

    public static Point Parse(string point)
    {
        var x = point.Split(',')[0];
        var y = point.Split(',')[1];

        return new Point(int.Parse(x), int.Parse(y));
    }

    public Vector3 ToVector()
    {
        return new Vector3(m_x, m_y, 0f);
    }

    public static bool TryParse(string point, out Point result)
    {
        try
        {
            Point p = Parse(point);
            result = p;
            return true;
        }
        catch
        {
            result = Point.InvalidPoint;
            return false;
        }
    }

    public static Point operator +(Point lhs, Point rhs)
    {
        return new Point(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Point operator -(Point lhs, Point rhs)
    {
        return new Point(lhs.x - rhs.x, lhs.y - rhs.y);
    }

    public override bool Equals(object obj)
    {
        if (obj is Point)
            return ((Point)obj).x == x && ((Point)obj).y == y;
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Point lhs, Point rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Point lhs, Point rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static float Distance(Point lhs, Point rhs)
    {
        return Mathf.Sqrt(Mathf.Pow(lhs.x - rhs.x, 2) + Mathf.Pow(lhs.y - rhs.y, 2));
    }

    public override string ToString()
    {
        return $"{x},{y}";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref m_x);
        serializer.SerializeValue(ref m_y);
    }
}

public class Tile : MonoBehaviour
{
    public HashSet<GameObject> Objects = new HashSet<GameObject>();

    public float Weight;
    public MovementBlockFlag BlocksMovementFlags;
    public bool BlocksVision;

    bool m_IsVisible = false;
    SpriteRenderer m_SpriteRenderer;
    Color m_Grey;
    Color m_Invisible;

    public void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Grey = new Color(0.2f, 0.2f, 0.2f);
        m_Invisible = new Color(1f, 1f, 1f, 0f);
        m_SpriteRenderer.color = m_Grey;
        m_IsVisible = false;
    }

    void Interact(GameEvent gameEvent)
    {
        var source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        var attackMe = GameEventPool.Get(GameEventId.HostileInteraction)
            .With(EventParameter.Target, gameObject);
        source.FireEvent(attackMe);
        attackMe.Release();
    }

    public void AddObject(GameObject obj)
    {
        Objects.Add(obj);
        Debug.LogError($"{obj.name} added to tile {transform.position.x}, {transform.position.y}");
        SetVisibility(m_IsVisible);
        //obj.gameObject.SetActive(true);
        Services.Coroutine.InvokeCoroutine(Recalculate());
    }

    public void SetVisibility(bool isVisible)
    {
        m_IsVisible = isVisible;

        if (isVisible)
        {
            m_SpriteRenderer.color = Color.white;
            foreach (var obj in Objects)
                obj.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            m_SpriteRenderer.color = m_Grey;
            foreach (var obj in Objects)
                obj.GetComponent<SpriteRenderer>().color = m_Invisible;
        }
    }


    public void RemoveObject(GameObject obj)
    {
        Objects.Remove(obj);
        //obj.gameObject.SetActive(false);
        Services.Coroutine.InvokeCoroutine(Recalculate());
    }

    public void FireEvent(GameObject source, GameEvent gameEvent)
    {
        if (Objects.Count == 0)
        {
            var hostileInteraction = GameEventPool.Get(GameEventId.HostileInteraction)
                .With(EventParameter.Target, gameObject);
            source.FireEvent(hostileInteraction);
            hostileInteraction.Release();
        }
        else
        {
            var objects = new HashSet<GameObject>(Objects);
            foreach (var go in objects)
                go.FireEvent(gameEvent);
        }
    }

    IEnumerator Recalculate()
    {
        yield return null;

        var tileData = GameEventPool.Get(GameEventId.CalculateTileFlags)
            .With(EventParameter.Weight, 1f)
            .With(EventParameter.BlocksMovementFlags, MovementBlockFlag.None)
            .With(EventParameter.BlocksVision, false);

        foreach (var go in Objects)
            go.FireEvent(tileData);

        Weight = tileData.GetValue<float>(EventParameter.Weight);
        BlocksMovementFlags = tileData.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        BlocksVision = tileData.GetValue<bool>(EventParameter.BlocksVision);

        tileData.Release();

        SetVisibility(m_IsVisible);
    }
}
