using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonGenerationResult
{
    public List<string> Walls = new List<string>();
    public List<string> Entities = new List<string>();
    public List<Room> RoomData = new List<Room>();
    public List<Point> TilePoints = new List<Point>();
    public List<bool> TileHasBeenVisited = new List<bool>();
    public int StairsDownRoomIndex;

    public void ClearData()
    {
        Walls.Clear();
        Entities.Clear();
        RoomData.Clear();
        TilePoints.Clear();
        TileHasBeenVisited.Clear();
    }
}

public interface IDungeonGenerator
{
    DungeonGenerationResult GenerateDungeon(DungeonMetaData metaData);
    void Clean();
    List<Room> Rooms { get; }
}
