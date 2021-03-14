using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonGenerator
{
    //void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap);
    //void GenerateDungeon(IMapNode[,] map);
    void GenerateDungeon(int rows, int columns);
    List<Room> Rooms { get; }
}
