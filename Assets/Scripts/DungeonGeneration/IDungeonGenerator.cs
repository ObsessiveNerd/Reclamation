using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonGenerationResult
{
    public List<string> Entities = new List<string>();
    public List<Room> RoomData = new List<Room>();
    public int StairsDownRoomIndex;
}

public interface IDungeonGenerator
{
    DungeonGenerationResult GenerateDungeon(DungeonMetaData metaData);
    void Clean();
    List<Room> Rooms { get; }
}
