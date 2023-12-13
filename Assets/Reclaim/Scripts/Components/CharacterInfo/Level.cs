using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : EntityComponent
{
    public int CurrentLevel;
    public int CurrentExp;

    int ExpToNextLevel
    {
        get
        {
            return (CurrentLevel + 1) * 25;
        }
    }

    public Level(int currentLevel, int currentExp)
    {
        CurrentLevel = currentLevel;
        CurrentExp = currentExp;
    }

    public override void Init(GameObject self)
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
            Debug.LogWarning($"Exp gained: {gameEvent.GetValue<int>(EventParameter.Exp)}");

            int expGained = gameEvent.GetValue<int>(EventParameter.Exp);

            //Shared player experience.  This should honestly be it's own component on the player, but this is a test
            if (!gameEvent.HasParameter(EventParameter.Flag))
            {
                if (Services.PlayerManagerService.GetAllPlayers().Contains(Self))
                {
                    foreach (var otherPlayer in Services.PlayerManagerService.GetAllPlayers())
                    {
                        if (otherPlayer != Self)
                        {
                            GameEvent giveExp = GameEventPool.Get(GameEventId.GainExperience)
                                        .With(EventParameter.Flag, true)
                                        .With(EventParameter.Exp, Mathf.Max(1, (int)((float)expGained * 0.1f)));
                            otherPlayer.FireEvent(giveExp).Release();
                        }
                    }
                }
            }

            CurrentExp += expGained;
            if(CurrentExp >= ExpToNextLevel)
            {
                do
                {
                    CurrentExp = CurrentExp - ExpToNextLevel;
                    CurrentLevel++;
                    GameEvent e = GameEventPool.Get(GameEventId.LevelUp)
                                    .With(EventParameter.Level, CurrentLevel);
                    FireEvent(Self, e).Release();

                } while (CurrentExp >= ExpToNextLevel);
            }
        }

        if(gameEvent.ID == GameEventId.Died)
        {
            GameEvent giveExp = GameEventPool.Get(GameEventId.GainExperience)
                                    .With(EventParameter.Exp, CurrentLevel * 3);

            var expGainer = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.DamageSource));
            expGainer.FireEvent(giveExp).Release();
        }

        if(gameEvent.ID == GameEventId.GetLevel)
        {
            gameEvent.Paramters[EventParameter.Level] = CurrentLevel;
        }

        if(gameEvent.ID == GameEventId.GetExperience)
        {
            gameEvent.Paramters[EventParameter.Value] = CurrentExp;
            gameEvent.Paramters[EventParameter.MaxValue] = ExpToNextLevel;
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
                if(!string.IsNullOrEmpty(value))
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
