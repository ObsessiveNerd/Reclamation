using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shadowcasting : IFovAlgorithm
{
    int m_Range;
    Point m_SourcePoint;
    List<Point> m_VisiblePoints;
    List<int> m_VisibleOctants = new List<int>() { 1,2,3,4,5,6,7,8 };

    public List<Point> GetVisibleTiles(GameObject source, int range, List<int> octants = null)
    {
        if (source == null)
            return new List<Point>();

        if (octants == null)
            octants = m_VisibleOctants;

        m_Range = range;
        m_SourcePoint = source.GetComponent<Position>().component.Point;

        m_VisiblePoints = new List<Point>();
        m_VisiblePoints.Add(m_SourcePoint);
        foreach (int octant in octants)
            ScanOctant(1, octant, 1.0, 0.0);
        m_VisiblePoints = m_VisiblePoints.Distinct(new PointComparer()).ToList();

        return m_VisiblePoints;
    }

    //TODO: we might need another implementation of this, at the very least we need to figure out something to let us mark the wall tiles as visible
    protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
    {
        int visrange2 = m_Range * m_Range;
        int x = 0;
        int y = 0;

        switch (pOctant)
        {

            case 1: //nnw
                y = m_SourcePoint.y - pDepth;
                if (y < 0)
                    return;

                x = m_SourcePoint.x - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (x < 0)
                    x = 0;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, false) >= pEndSlope)
                {
                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {
                        if (TileIsBlocking(x, y)) //current cell blocked
                        {
                            if (x - 1 >= 0 && !TileIsBlocking(x - 1, y)) //prior cell within range AND open...
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, false));
                            //else
                            m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {

                            if (x - 1 >= 0 && TileIsBlocking(x - 1, y)) //prior cell within range AND open...
                                                                        //..adjust the startslope
                                pStartSlope = GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    x++;
                }
                x--;
                break;

            case 2: //nne

                y = m_SourcePoint.y - pDepth;
                if (y < 0)
                    return;

                x = m_SourcePoint.x + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (x >= Services.Map.Width)
                    x = Services.Map.Width - 1;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, false) <= pEndSlope)
                {
                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {
                        if (TileIsBlocking(x, y))
                        {
                            if (x + 1 < Services.Map.Width && !TileIsBlocking(x + 1, y))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, false));
                            else
                                m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (x + 1 < Services.Map.Width && TileIsBlocking(x + 1, y))
                                pStartSlope = -GetSlope(x + 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    x--;
                }
                x++;
                break;

            case 3:

                x = m_SourcePoint.x + pDepth;
                if (x >= Services.Map.Width)
                    return;

                y = m_SourcePoint.y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (y < 0)
                    y = 0;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, true) <= pEndSlope)
                {

                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (y - 1 >= 0 && !TileIsBlocking(x, y - 1))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, true));
                            //else
                            m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (y - 1 >= 0 && TileIsBlocking(x, y - 1))
                                pStartSlope = -GetSlope(x + 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, true);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    y++;
                }
                y--;
                break;

            case 4:

                x = m_SourcePoint.x + pDepth;
                if (x >= Services.Map.Width)
                    return;

                y = m_SourcePoint.y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (y >= Services.Map.Height)
                    y = Services.Map.Height - 1;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, true) >= pEndSlope)
                {

                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (y + 1 < Services.Map.Height && !TileIsBlocking(x, y + 1))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, true));
                            else
                                m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (y + 1 < Services.Map.Height && TileIsBlocking(x, y + 1))
                                pStartSlope = GetSlope(x + 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, true);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    y--;
                }
                y++;
                break;

            case 5:

                y = m_SourcePoint.y + pDepth;
                if (y >= Services.Map.Height)
                    return;

                x = m_SourcePoint.x + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (x >= Services.Map.Width)
                    x = Services.Map.Width - 1;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, false) >= pEndSlope)
                {
                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (x + 1 < Services.Map.Height && !TileIsBlocking(x + 1, y))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false));
                            //else
                            m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (x + 1 < Services.Map.Height
                                    && TileIsBlocking(x + 1, y))
                                pStartSlope = GetSlope(x + 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, false);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    x--;
                }
                x++;
                break;

            case 6:

                y = m_SourcePoint.y + pDepth;
                if (y >= Services.Map.Height)
                    return;

                x = m_SourcePoint.x - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (x < 0)
                    x = 0;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, false) <= pEndSlope)
                {
                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (x - 1 >= 0 && !TileIsBlocking(x - 1, y))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, false));
                            else
                                m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (x - 1 >= 0
                                    && TileIsBlocking(x - 1, y))
                                pStartSlope = -GetSlope(x - 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, false);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    x++;
                }
                x--;
                break;

            case 7:

                x = m_SourcePoint.x - pDepth;
                if (x < 0)
                    return;

                y = m_SourcePoint.y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (y >= Services.Map.Height)
                    y = Services.Map.Height - 1;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, true) <= pEndSlope)
                {

                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (y + 1 < Services.Map.Height && !TileIsBlocking(x, y + 1))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, true));
                            //else
                            m_VisiblePoints.Add(new Point(x, y));
                        }
                        else
                        {
                            if (y + 1 < Services.Map.Height && TileIsBlocking(x, y + 1))
                                pStartSlope = -GetSlope(x - 0.5, y + 0.5, m_SourcePoint.x, m_SourcePoint.y, true);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    y--;
                }
                y++;
                break;

            case 8: //wnw

                x = m_SourcePoint.x - pDepth;
                if (x < 0)
                    return;

                y = m_SourcePoint.y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                if (y < 0)
                    y = 0;

                while (GetSlope(x, y, m_SourcePoint.x, m_SourcePoint.y, true) >= pEndSlope)
                {

                    if (GetDistance(x, y, m_SourcePoint.x, m_SourcePoint.y) <= visrange2)
                    {

                        if (TileIsBlocking(x, y))
                        {
                            if (y - 1 >= 0 && !TileIsBlocking(x, y - 1))
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, true));
                            else
                                m_VisiblePoints.Add(new Point(x, y));

                        }
                        else
                        {
                            if (y - 1 >= 0 && TileIsBlocking(x, y - 1))
                                pStartSlope = GetSlope(x - 0.5, y - 0.5, m_SourcePoint.x, m_SourcePoint.y, true);

                            m_VisiblePoints.Add(new Point(x, y));
                        }
                    }
                    y++;
                }
                y--;
                break;
        }


        if (x < 0)
            x = 0;
        else if (x >= Services.Map.Width)
            x = Services.Map.Width - 1;

        if (y < 0)
            y = 0;
        else if (y >= Services.Map.Height)
            y = Services.Map.Height - 1;

        if (pDepth < m_Range & !TileIsBlocking(x, y))
            ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);

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
        return Services.Map.GetTile(new Point(x, y)).BlocksVision;
    }
}
