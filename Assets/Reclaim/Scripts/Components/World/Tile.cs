using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//public class PointComparer : IEqualityComparer<Point>
//{
//    public bool Equals(Point x, Point y)
//    {
//        return x == y;
//    }

//    public int GetHashCode(Point obj)
//    {
//        return obj.GetHashCode();
//    }
//}

[Serializable]
public struct Point
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

    public static Point Parse(string point)
    {
        var x = point.Split(',')[0];
        var y = point.Split(',')[1];

        return new Point(int.Parse(x), int.Parse(y));
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
}

public class Tile : MonoBehaviour
{
    HashSet<GameObject> Objects = new HashSet<GameObject>();

    public float Weight;
    public bool BlocksMovement;
    public bool BlocksVision;

    public void AddObject(GameObject obj)
    {
        Objects.Add(obj);
        //Recheck Weight, Movement, and Vision
    }

    public void RemoveObject(GameObject obj)
    {
        Objects.Remove(obj);
        //Recheck Weight, Movement, and Vision
    }
}
