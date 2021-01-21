using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    public string Info;
    public List<string> Entites = new List<string>();
    public LevelInfo(string info)
    {
        Info = info;
    }
}

[Serializable]
public class SaveData
{
    public string Seed;
    public LevelInfo LevelInfo;

    public SaveData(string seed, string levelInfo)
    {
        Seed = seed;
        LevelInfo = new LevelInfo(levelInfo);
    }
}

public class SaveSystem : MonoBehaviour
{
    public const string kSaveDataPath = "SaveData";
    public void Save()
    {
        StartCoroutine(SaveAsync());
    }

    IEnumerator SaveAsync()
    {
        World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.PauseTime));
        yield return null;
        SaveData data = new SaveData("0", "[0]");

        List<IEntity> entities = (List<IEntity>)World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.GetEntities, new KeyValuePair<string, object>(EventParameters.Value, new List<IEntity>()))).
                                    Paramters[EventParameters.Value];

        foreach (IEntity entity in entities)
            data.LevelInfo.Entites.Add(entity.Serialize());

        string jsonData = JsonUtility.ToJson(data, true);
        string path = $"{kSaveDataPath}/{data.Seed}/data.save";
        File.WriteAllText(path, jsonData);
        World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.UnPauseTime));
    }

    public static void Load(string path)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        foreach (var entity in data.LevelInfo.Entites)
            Spawner.Restore(EntityFactory.ParseEntityData(entity));
    }
}
