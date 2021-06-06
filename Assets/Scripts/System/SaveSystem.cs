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
    public List<string> Events;
    public SaveData(int seed)
    {
        Seed = seed;
        Events = new List<string>();
    }
}

[Serializable]
public class SerializedLevelData
{
    public List<string> Entities = new List<string>();
    public List<Room> RoomData = new List<Room>();
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
        m_Data = new SaveData(World.Instance.Seed);

        string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        if (!File.Exists(path))
            return;

        using (var reader = new StreamReader(path))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                m_Data.Events.Add(line);
            }
        }

        File.WriteAllText($"{kSaveDataPath}/{World.Instance.Seed}/data.save", JsonUtility.ToJson(m_Data));
    }

    public void SaveLevel(SerializedLevelData levelData, int level)
    {
        string path = $"{kSaveDataPath}/{World.Instance.Seed}/{level}";
        if(Directory.Exists(path))
            Directory.Delete(path, true);

        Directory.CreateDirectory(path);
        File.WriteAllText($"{path}/data.dat", JsonUtility.ToJson(levelData));
    }

    public SerializedLevelData LoadLevel(int level)
    {
        string path = $"{kSaveDataPath}/{World.Instance.Seed}/{level}";
        if (!Directory.Exists(path))
            return null;
        return JsonUtility.FromJson<SerializedLevelData>(File.ReadAllText($"{path}/data.dat"));
    }

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.AppendAllText(path, JsonUtility.ToJson(new GameEventSerializable(targetId, gameEvent)));
        File.AppendAllText(path, "\n");
    }

    public void Load(string path)
    {
        //StartCoroutine(LoadInternal(path));

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        World.Instance.InitWorld(data.Seed);
        return;

        foreach (string eventString in data.Events)
        {
            GameEventSerializable ges = JsonUtility.FromJson<GameEventSerializable>(eventString);
            string targetID = ges.TargetEntityId;
            GameEvent ge = ges.CreateGameEvent();
            IEntity target = EntityQuery.GetEntity(targetID);

            target.FireEvent(target, ge);
            EventBuilder builder = new EventBuilder(GameEventId.ProgressTimeUntilIdHasTakenTurn)
                                    .With(EventParameters.Entity, targetID);
            World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
        }
    }

    internal IEnumerator LoadInternal(string path)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        World.Instance.InitWorld(data.Seed);

        foreach (string eventString in data.Events)
        {
            GameEventSerializable ges = JsonUtility.FromJson<GameEventSerializable>(eventString);
            string targetID = ges.TargetEntityId;
            GameEvent ge = ges.CreateGameEvent();
            IEntity target = EntityQuery.GetEntity(targetID);

            target.FireEvent(target, ge);
            EventBuilder builder = new EventBuilder(GameEventId.ProgressTimeUntilIdHasTakenTurn)
                                    .With(EventParameters.Entity, targetID);
            World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());

            yield return new WaitForSeconds(1.0f);
        }
    }
}
