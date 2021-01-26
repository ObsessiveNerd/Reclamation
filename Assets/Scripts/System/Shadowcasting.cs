using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadowcasting : IFovAlgorithm
{
    int m_Range;
    IEntity m_Source;
    Point m_SourcePoint;
    List<Point> m_VisiblePoints;
    List<int> m_VisibleOctants = new List<int>() { 1, /*2, 3, 4, 5, 6, 7, 8*/ };

    public List<Point> GetVisibleTiles(IEntity source, int range)
    {
        m_Range = range;
        m_Source = source;
        m_SourcePoint = (Point)source.FireEvent(World.Instance.Self, new GameEvent(GameEventId.GetEntityLocation, new KeyValuePair<string, object>(EventParameters.Entity, source),
                                                                                                            new KeyValuePair<string, object>(EventParameters.TilePosition, null)))
            .Paramters[EventParameters.TilePosition];
        m_VisiblePoints = new List<Point>();
        m_VisiblePoints.Add(m_SourcePoint);
        foreach (int octant in m_VisibleOctants)
            ScanOctant(1, octant, 1.0, 0.0);
        return m_VisiblePoints;
    }

    void ScanOctant(int depth, int octant, double startSlope, double endSlope)
    {
        int visSquared = m_Range * m_Range;
        int x = 0;
        int y = 0;

        switch(octant)
        {
            case 1:
                y = m_SourcePoint.y - depth;
                if (y < 0)
                    return;
                x = m_SourcePoint.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
                if (x < 0)
                    x = 0;

                while(GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, false) >= endSlope)
                {
                    if(GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visSquared)
                    {
                        if(TileIsBlocking(x, y))
                        {
                            if (x - 1 >= 0 && !TileIsBlocking(x - 1, y))
                                ScanOctant(depth + 1, octant, startSlope, GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false));
                        }
                        else
                        {
                            if (x - 1 >= 0 && TileIsBlocking(x - 1, y))
                                startSlope = GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false);
                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    x++;
                }
                x--;
                break;
        }

        if (x < 0)
            x = 0;
        else if (x >= World.Instance.MapColumns)
            x = World.Instance.MapColumns - 1;

        if (y < 0)
            y = 0;
        else if (y >= World.Instance.MapRows)
            y = World.Instance.MapRows - 1;

        if (depth < m_Range && !TileIsBlocking(x, y))
            ScanOctant(depth + 1, octant, startSlope, endSlope);
    }

    double GetSlope(double x, double y, int sourceX, int sourceY, bool invert)
    {
        if (invert)
            return (y - sourceY) / (x - sourceX);
        else
            return (x - sourceX) / (y - sourceY);
    }

    double GetDistance(double x, double y, int sourceX, int sourceY)
    {
        return ((x - sourceX) * (x - sourceX)) + ((y - sourceY) * (y - sourceY));
    }

    bool TileIsBlocking(int x, int y)
    {
        bool tileIsBlocking = (bool)m_Source.FireEvent(World.Instance.Self, new GameEvent(GameEventId.IsTileBlocking, new KeyValuePair<string, object>(EventParameters.TilePosition, new Point(x, y)),
                                                                                                                        new KeyValuePair<string, object>(EventParameters.Value, false))).Paramters[EventParameters.Value];
        return tileIsBlocking;
    }
}
