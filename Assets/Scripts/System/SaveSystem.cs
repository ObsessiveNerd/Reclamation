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

    //public List<string> Events;
    public SaveData(int seed)
    {
        Seed = seed;
        //Events = new List<string>();
    }
}

public class SaveSystem : MonoBehaviour
{
    public const string kSaveDataPath = "SaveData";
    public string CurrentSaveName;
    SaveData m_Data;

    public static SaveSystem Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

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
        m_Data = new SaveData(World.Instance.Seed);

        EventBuilder getCurrentLevel = new EventBuilder(GameEventId.GetCurrentLevel)
                                        .With(EventParameters.Level, -1);
        m_Data.CurrentDungeonLevel = World.Instance.Self.FireEvent(getCurrentLevel.CreateEvent()).GetValue<int>(EventParameters.Level);
        m_Data.SaveName = CurrentSaveName = saveName;

        //string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        //Directory.CreateDirectory(Path.GetDirectoryName(path));
        //if (!File.Exists(path))
        //    return;

        //using (var reader = new StreamReader(path))
        //{
        //    while (!reader.EndOfStream)
        //    {
        //        string line = reader.ReadLine();
        //        m_Data.Events.Add(line);
        //    }
        //}
        World.Instance.Self.FireEvent(new GameEvent(GameEventId.SaveLevel));
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

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        //string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        //Directory.CreateDirectory(Path.GetDirectoryName(path));
        //File.AppendAllText(path, JsonUtility.ToJson(new GameEventSerializable(targetId, gameEvent)));
        //File.AppendAllText(path, "\n");
    }

    public void Load(string path)
    {
        //StartCoroutine(LoadInternal(path));

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        CurrentSaveName = data.SaveName;
        World.Instance.Self.FireEvent(new GameEvent(GameEventId.LoadLevel, new KeyValuePair<string, object>(EventParameters.Level, data.CurrentDungeonLevel)));
        World.Instance.InitWorld(data.Seed);

        //foreach (string eventString in data.Events)
        //{
        //    GameEventSerializable ges = JsonUtility.FromJson<GameEventSerializable>(eventString);
        //    string targetID = ges.TargetEntityId;
        //    GameEvent ge = ges.CreateGameEvent();
        //    IEntity target = EntityQuery.GetEntity(targetID);

        //    target.FireEvent(target, ge);
        //    EventBuilder builder = new EventBuilder(GameEventId.ProgressTimeUntilIdHasTakenTurn)
        //                            .With(EventParameters.Entity, targetID);
        //    World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
        //}
    }

    //internal IEnumerator LoadInternal(string path)
    //{
    //    SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    //    World.Instance.InitWorld(data.Seed);

    //    foreach (string eventString in data.Events)
    //    {
    //        GameEventSerializable ges = JsonUtility.FromJson<GameEventSerializable>(eventString);
    //        string targetID = ges.TargetEntityId;
    //        GameEvent ge = ges.CreateGameEvent();
    //        IEntity target = EntityQuery.GetEntity(targetID);

    //        target.FireEvent(target, ge);
    //        EventBuilder builder = new EventBuilder(GameEventId.ProgressTimeUntilIdHasTakenTurn)
    //                                .With(EventParameters.Entity, targetID);
    //        World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());

    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}
}
