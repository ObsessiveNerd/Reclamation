using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    public List<string> Entites = new List<string>();
    public LevelInfo(List<string> entities)
    {
        Entites = entities;
    }
}

[Serializable]
public class SaveData
{
    public int Seed;
    public int CurrentLevelIndex;
    public List<LevelInfo> LevelInfo = new List<LevelInfo>();

    public SaveData(int seed)
    {
        Seed = seed;
    }
}

public class SaveSystem : MonoBehaviour
{
    public const string kSaveDataPath = "SaveData";
    SaveData m_Data;

    public static SaveSystem Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Save()
    {
        //StartCoroutine(SaveAsync());
    }

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        string path = $"{kSaveDataPath}/test/data.save";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.AppendAllText(path, JsonUtility.ToJson(new GameEventSerializable(targetId, gameEvent)));
        File.AppendAllText(path, "\n");
    }

    public void SetSaveDataSeed(int seed)
    {
        m_Data = new SaveData(seed);
    }

    public void StoreLevelInfo(List<string> entities)
    {
        m_Data.LevelInfo.Add(new LevelInfo(entities));
    }

    int m_LevelIndex = 0;
    public void MovingToNewLevel()
    {
        m_LevelIndex++;
        m_Data.CurrentLevelIndex = m_LevelIndex;
    }

    //IEnumerator SaveAsync(bool movingToNewLevel = false)
    //{
    //    World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.PauseTime));
    //    yield return null;

    //    List<IEntity> entities = (List<IEntity>)World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.GetEntities, new KeyValuePair<string, object>(EventParameters.Value, new List<IEntity>()))).
    //                                Paramters[EventParameters.Value];

    //    List<string> serializedEntities = new List<string>();
    //    foreach (IEntity entity in entities)
    //        serializedEntities.Add(entity.Serialize());

    //    StoreLevelInfo(serializedEntities);

    //    string jsonData = JsonUtility.ToJson(m_Data, true);
    //    string path = $"{kSaveDataPath}/{m_Data.Seed}/data.save";
    //    Directory.CreateDirectory(Path.GetDirectoryName(path));
    //    File.WriteAllText(path, jsonData);
    //    World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.UnPauseTime));

    //    if (movingToNewLevel)
    //        MovingToNewLevel();
    //}

    public static void Load(string path)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        World.Instance.Seed = data.Seed;

        foreach (var entity in data.LevelInfo[data.CurrentLevelIndex].Entites)
            Spawner.Restore(EntityFactory.ParseEntityData(entity));
        Instance.SetSaveDataSeed(data.Seed);
    }
}
