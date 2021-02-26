using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonGenerator
{
    void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap);
    List<Room> Rooms { get; }
}
