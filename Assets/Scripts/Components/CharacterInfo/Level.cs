using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Component
{
    public int CurrentLevel;
    public int CurrentExp;

    int m_ExpToNextLevel;

    public Level(int currentLevel, int currentExp)
    {
        CurrentLevel = currentLevel;
        CurrentExp = currentExp;

        m_ExpToNextLevel = (CurrentLevel + 1) * 10;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GainExperience);
        RegisteredEvents.Add(GameEventId.Died);
        RegisteredEvents.Add(GameEventId.GetExperience);
        RegisteredEvents.Add(GameEventId.GetLevel);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GainExperience)
        {
            Debug.LogWarning($"Exp gained: {gameEvent.GetValue<int>(EventParameters.Exp)}");

            CurrentExp += gameEvent.GetValue<int>(EventParameters.Exp);
            if(CurrentExp >= m_ExpToNextLevel)
            {
                CurrentExp = CurrentExp - m_ExpToNextLevel;
                CurrentLevel++;
                GameEvent e = GameEventPool.Get(GameEventId.LevelUp)
                                    .With(EventParameters.Level, CurrentLevel);
                FireEvent(Self, e).Release();
            }
        }

        if(gameEvent.ID == GameEventId.Died)
        {
            GameEvent giveExp = GameEventPool.Get(GameEventId.GainExperience)
                                    .With(EventParameters.Exp, CurrentLevel * 3);

            EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.DamageSource)).FireEvent(giveExp).Release();
        }

        if(gameEvent.ID == GameEventId.GetLevel)
        {
            gameEvent.Paramters[EventParameters.Level] = CurrentLevel;
        }

        if(gameEvent.ID == GameEventId.GetExperience)
        {
            gameEvent.Paramters[EventParameters.Value] = CurrentExp;
            gameEvent.Paramters[EventParameters.MaxValue] = m_ExpToNextLevel;
        }
    }
}

public class DTO_Level : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int currentLevel = 0;
        int exp = 0;

        string[] kvpairs = data.Split(',');
        foreach(var kvp in kvpairs)
        {
            string key = kvp.Split('=')[0];
            string value = kvp.Split('=')[1];

            if (key == "CurrentLevel")
                currentLevel = int.Parse(value);
            if (key == "CurrentExp")
                exp = int.Parse(value);
        }

        Component = new Level(currentLevel, exp);
    }

    public string CreateSerializableData(IComponent component)
    {
        Level l = (Level)component;
        return $"{nameof(Level)}: {nameof(l.CurrentLevel)}={l.CurrentLevel}, {nameof(l.CurrentExp)}={l.CurrentExp}";
    }
}
