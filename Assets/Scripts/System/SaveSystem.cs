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

    public static void LogEvent(string targetId, GameEvent gameEvent)
    {
        string path = $"{kSaveDataPath}/{World.Instance.Seed}/tmp_event_log.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.AppendAllText(path, JsonUtility.ToJson(new GameEventSerializable(targetId, gameEvent)));
        File.AppendAllText(path, "\n");
    }

    //public void SetSaveDataSeed(int seed)
    //{
       
    //}

    //public void StoreLevelInfo(List<string> entities)
    //{
    //    m_Data.LevelInfo.Add(new LevelInfo(entities));
    //}

    //int m_LevelIndex = 0;
    //public void MovingToNewLevel()
    //{
    //    m_LevelIndex++;
    //    m_Data.CurrentLevelIndex = m_LevelIndex;
    //}

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
        Instance.StartCoroutine(Instance.LoadInternal(path));
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
