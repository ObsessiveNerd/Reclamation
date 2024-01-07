using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject Tile;

    Tile[,] tiles;

    public void Awake()
    {
        Services.Register(this);
        tiles = new Tile[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                tiles[i, j] = Instantiate(Tile, new Vector3(i, j), Quaternion.identity, transform).GetComponent<Tile>();
    }

    public Tile GetTile(Point point)
    {
        return GetTile(point.x, point.y);
    }
    public Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public  Point GetTilePointInDirection(Point basePoint, MoveDirection direction)
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
