using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkEntityData
{
    public string EntityData;
    public List<string> Blueprints;
    public int CurrentLevel;

    public NetworkEntityData(string entityData, List<string> blueprints, int currentLevel)
    {
        EntityData = entityData;
        Blueprints = blueprints;
        CurrentLevel = currentLevel;
    }
}

public struct NetworkIdData
{
    public string NetworkId;
    public bool isHost;
}

public struct NetworkDungeonData
{
    public string SaveFile;
    public List<string> LevelDatas;
    public List<string> TempBlueprints;

    public NetworkDungeonData(string saveFile, List<string> levelDatas, List<string> tempBlueprints)
    {
        SaveFile = saveFile;
        LevelDatas = levelDatas;
        TempBlueprints = tempBlueprints;
    }
}
