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
    public void Save()
    {
        StartCoroutine(SaveAsync());
    }

    IEnumerator SaveAsync()
    {
        World.Instance.StopTime = true;
        yield return null;
        SaveData data = new SaveData("0", "[0]");
        foreach (IEntity entity in World.Instance.GetEntities())
            data.LevelInfo.Entites.Add(entity.Serialize());

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText("Assets/TestSave.sv", jsonData);
        World.Instance.StopTime = false;
    }

    public static void Load(string path)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        World.Instance.Seed = data.Seed;
        foreach (var entity in data.LevelInfo.Entites)
            Spawner.Restore(EntityFactory.ParseEntityData(entity));
    }
}
