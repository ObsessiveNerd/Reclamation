using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DungeonGenerationResult
{

}

public interface IDungeonGenerator
{
    //void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap);
    //void GenerateDungeon(IMapNode[,] map);
    DungeonGenerationResult GenerateDungeon(DungeonMetaData metaData);
    void Clean();
    List<Room> Rooms { get; }
}
