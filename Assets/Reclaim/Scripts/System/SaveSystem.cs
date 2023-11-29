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
    string m_LoadPath;
    SaveData m_Data;

    public string LoadPath => m_LoadPath;

    public string CurrentSavePath
    {
        get
        {
            return $"{m_LoadPath}/data.save";
        }
    }

    //public static GameSaveSystem Instance { get; set; }

    public GameSaveSystem(string loadPath)
    {
        m_LoadPath = loadPath;
        CurrentSaveName = Path.GetFileName(loadPath);
        if (File.Exists(CurrentSavePath))
        {
            m_Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(CurrentSavePath));
            RecRandom.InitRecRandom(m_Data.Seed);
        }
        else
            RecRandom.InitRecRandom(DateTime.Now.Millisecond);
    }

    public void Save()
    {
        if(!string.IsNullOrEmpty(CurrentSaveName))
            Save(CurrentSaveName);
    }

    public void UpdateSaveFromNetwork(NetworkDungeonData data)
    {
        Directory.CreateDirectory(LoadPath);

        if(!string.IsNullOrEmpty(data.SaveFile))
        {
            var save = JsonUtility.FromJson<SaveData>(data.SaveFile);
            RecRandom.InitRecRandom(save.Seed);
            File.WriteAllText(CurrentSavePath, data.SaveFile);
        }

        for (int i = 0; i < data.LevelDatas.Count; i++)
        {
            if (string.IsNullOrEmpty(data.LevelDatas[i]))
                continue;

            string levelDir =$"{m_LoadPath}/{data.LevelsToUpdate[i]}";
            Directory.CreateDirectory(levelDir);
            if(File.Exists($"{levelDir}/data.dat"))
            {
                var remoteDateModified = DateTime.Parse(data.FileDateModified[i]);

                //Local file is newer, so don't overwrite
                if (File.GetLastWriteTime($"{levelDir}/data.dat") >= remoteDateModified)
                    continue;
            }

            File.WriteAllText($"{levelDir}/data.dat", data.LevelDatas[i]);
        }

        foreach(var bp in data.TempBlueprints)
        {
            string name = EntityFactory.GetEntityNameFromBlueprintFormatting(bp);
            EntityFactory.CreateTemporaryBlueprint(name.Split(',')[1], bp);
        }
    }

    public void MoveSaveData(string newPath)
    {
        newPath = $"{kSaveDataPath}/{newPath}";
        if (LoadPath == newPath)
            return;

        if (Directory.Exists(LoadPath))
        {
            Directory.CreateDirectory(newPath);
            Directory.Move(LoadPath, newPath);
        }
        m_LoadPath = newPath;
        CurrentSaveName = Path.GetFileName(newPath);
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
        if(m_Data == null)
            m_Data = new SaveData(m_Seed);

        m_Data.CurrentDungeonLevel = m_CurrentLevel;
        m_Data.SaveName = CurrentSaveName = saveName;

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
        else if (LoadLevel(m_CurrentLevel) != null)
        {
            level = LoadLevel(m_CurrentLevel);
            level.ClearData();
            m_DungeonLevelMap.Add(m_CurrentLevel, level);
        }
        else
            return;

        foreach (var tile in m_Tiles.Values)
        {
            GameEvent serializeTile = GameEventPool.Get(GameEventId.SerializeTile)
                                         .With(EventParameter.Value, level);
            tile.SerializeTile(serializeTile);
            serializeTile.Release();
        }

        if (!Services.DungeonService.IsReady)
            return;

        foreach (Room room in Services.DungeonService.DungeonGenerator.Rooms)
            level.RoomData.Add(room);

        SaveLevel(level, m_CurrentLevel);
    }

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        GameEventSerializable ges = new GameEventSerializable(targetId, gameEvent);
        if(Services.NetworkService.IsConnected)
            Services.NetworkService.EmitEvent(ges);
    }

    public void Load(string path, bool overrideCurrentSaveName = true)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        if (overrideCurrentSaveName)
            CurrentSaveName = data.SaveName;
        else
            data.SaveName = CurrentSaveName;
        Services.DungeonService.GenerateDungeon(data.CurrentDungeonLevel, false);
    }
}
