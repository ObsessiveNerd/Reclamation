using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int Seed;
    public int CurrentDungeonLevel;
    public string SaveName;

    public SaveData(int seed)
    {
        Seed = seed;
    }
}

public class GameSaveSystem : GameService
{
    public const string kSaveDataPath = "SaveData";
    [HideInInspector]
    public string CurrentSaveName;
    SaveData m_Data;

    public static GameSaveSystem Instance { get; set; }

    public void Save()
    {
        if(!string.IsNullOrEmpty(CurrentSaveName))
            Save(CurrentSaveName);
    }

    public void CleanCurrentSave()
    {
        EntityFactory.Clean();
        string path = $"{kSaveDataPath}/{CurrentSaveName}";
        Directory.Delete(path, true);
        Directory.CreateDirectory(path);
    }

    public void Save(string saveName)
    {
        m_Data = new SaveData(m_Seed);

        GameEvent getCurrentLevel = GameEventPool.Get(GameEventId.GetCurrentLevel)
                                        .With(EventParameters.Level, -1);
        m_Data.CurrentDungeonLevel = m_CurrentLevel;
        m_Data.SaveName = CurrentSaveName = saveName;
        getCurrentLevel.Release();

        SaveCurrentLevel();
        Directory.CreateDirectory($"{kSaveDataPath}/{saveName}");
        File.WriteAllText($"{kSaveDataPath}/{saveName}/data.save", JsonUtility.ToJson(m_Data));
    }

    public void WriteData(string subPath, string data)
    {
        string path = $"{kSaveDataPath}/{CurrentSaveName}/{subPath}";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, data);
    }

    public void SaveLevel(DungeonGenerationResult levelData, int level)
    {
        string path = $"{kSaveDataPath}/{CurrentSaveName}/{level}";
        if(Directory.Exists(path))
            Directory.Delete(path, true);

        Directory.CreateDirectory(path);
        File.WriteAllText($"{path}/data.dat", JsonUtility.ToJson(levelData));
    }

    public DungeonGenerationResult LoadLevel(int level)
    {
        string path = $"{kSaveDataPath}/{CurrentSaveName}/{level}";
        if (!Directory.Exists(path))
            return null;
        return JsonUtility.FromJson<DungeonGenerationResult>(File.ReadAllText($"{path}/data.dat"));
    }

    void SaveCurrentLevel()
    {
        DungeonGenerationResult level = null;
        if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            level = m_DungeonLevelMap[m_CurrentLevel];
            level.ClearData();
        }
        else if (GameSaveSystem.Instance.LoadLevel(m_CurrentLevel) != null)
        {
            level = GameSaveSystem.Instance.LoadLevel(m_CurrentLevel);
            level.ClearData();
            m_DungeonLevelMap.Add(m_CurrentLevel, level);
        }
        else
            return;

        foreach (var tile in m_Tiles.Values)
        {
            GameEvent serializeTile = GameEventPool.Get(GameEventId.SerializeTile)
                                         .With(EventParameters.Value, level);
            tile.SerializeTile(serializeTile);
            serializeTile.Release();
        }

        foreach (Room room in Services.DungeonService.DungeonGenerator.Rooms)
            level.RoomData.Add(room);

        SaveLevel(level, m_CurrentLevel);
    }

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        //string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        //Directory.CreateDirectory(Path.GetDirectoryName(path));
        //File.AppendAllText(path, JsonUtility.ToJson(new GameEventSerializable(targetId, gameEvent)));
        //File.AppendAllText(path, "\n");
    }

    public void Load(string path)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        CurrentSaveName = data.SaveName;
        Services.DungeonService.GenerateDungeon(data.CurrentDungeonLevel, false);
    }
}
