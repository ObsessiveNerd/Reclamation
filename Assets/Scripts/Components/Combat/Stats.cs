using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    Str,
    Agi,
    Con,
    Wis,
    Int,
    Cha
}

public class Stats : Component
{
    public int Str;
    public int Agi;
    public int Con;
    public int Wis;
    public int Int;
    public int Cha;

    public int AttributePoints;

    public Stats(int Str, int Agi, int Con, int Wis, int Int, int Cha, int attributePoints)
    {
        SetStats(Str, Agi, Con, Wis, Int, Cha);
        AttributePoints = attributePoints;
        RegisteredEvents.Add(GameEventId.RollToHit);
        RegisteredEvents.Add(GameEventId.GetStat);
        RegisteredEvents.Add(GameEventId.GetStatRaw);
        RegisteredEvents.Add(GameEventId.LevelUp);
        RegisteredEvents.Add(GameEventId.BoostStat);
        RegisteredEvents.Add(GameEventId.GetAttributePoints);
    }

    void SetStats(int Str, int Agi, int Con, int Wis, int Int, int Cha)
    {
        this.Str = Str;
        this.Agi = Agi;
        this.Con = Con;
        this.Wis = Wis;
        this.Int = Int;
        this.Cha = Cha;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RollToHit)
        {
            int totalRoll = Dice.Roll("1d20");
            TypeWeapon weaponType = (TypeWeapon)gameEvent.Paramters[EventParameters.WeaponType];
            switch(weaponType)
            {
                case TypeWeapon.Melee:
                case TypeWeapon.StrSpell:
                    totalRoll += GetModifier(Str);
                    break;
                case TypeWeapon.Finesse:
                case TypeWeapon.Ranged:
                case TypeWeapon.AgiSpell:
                    totalRoll += GetModifier(Agi);
                    break;
                case TypeWeapon.MagicStaff:
                case TypeWeapon.IntSpell:
                    totalRoll += GetModifier(Int);
                    break;
                case TypeWeapon.ConSpell:
                    totalRoll += GetModifier(Con);
                    break;
                case TypeWeapon.WisSpell:
                    totalRoll += GetModifier(Wis);
                    break;
                case TypeWeapon.ChaSpell:
                    totalRoll += GetModifier(Cha);
                    break;
            }
            gameEvent.Paramters[EventParameters.RollToHit] = totalRoll;
        }
        else if(gameEvent.ID == GameEventId.GetStat)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            switch (s)
            {
                case Stat.Str:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Str);
                    break;
                case Stat.Agi:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Agi);
                    break;
                case Stat.Con:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Con);
                    break;
                case Stat.Wis:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Wis);
                    break;
                case Stat.Int:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Int);
                    break;
                case Stat.Cha:
                    gameEvent.Paramters[EventParameters.Value] = GetModifier(Cha);
                    break;
            }
        }
        else if(gameEvent.ID == GameEventId.GetStatRaw)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            switch (s)
            {
                case Stat.Str:
                    gameEvent.Paramters[EventParameters.Value] = Str;
                    break;
                case Stat.Agi:
                    gameEvent.Paramters[EventParameters.Value] = Agi;
                    break;
                case Stat.Con:
                    gameEvent.Paramters[EventParameters.Value] = Con;
                    break;
                case Stat.Wis:
                    gameEvent.Paramters[EventParameters.Value] = Wis;
                    break;
                case Stat.Int:
                    gameEvent.Paramters[EventParameters.Value] = Int;
                    break;
                case Stat.Cha:
                    gameEvent.Paramters[EventParameters.Value] = Cha;
                    break;
            }
        }
        else if(gameEvent.ID == GameEventId.LevelUp)
        {
            int newLevel = gameEvent.GetValue<int>(EventParameters.Level);
            if(newLevel % 2 == 0)
                AttributePoints += 2;
        }
        else if(gameEvent.ID == GameEventId.BoostStat)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            int amountToBoost = gameEvent.GetValue<int>(EventParameters.StatBoostAmount);

            switch (s)
            {
                case Stat.Str:
                    Str += amountToBoost;
                    break;
                case Stat.Agi:
                    Agi += amountToBoost;
                    break;
                case Stat.Con:
                    Con += amountToBoost;
                    break;
                case Stat.Wis:
                    Wis += amountToBoost;
                    break;
                case Stat.Int:
                    Int += amountToBoost;
                    break;
                case Stat.Cha:
                    Cha += amountToBoost;
                    break;
            }

            AttributePoints = Mathf.Max(0, AttributePoints - 1);
        }
        else if(gameEvent.ID == GameEventId.GetAttributePoints)
        {
            gameEvent.Paramters[EventParameters.AttributePoints] = AttributePoints;
        }
    }

    int GetModifier(int value)
    {
        return (value - 10) / 2;
    }
}

public class DTO_Stats : IDataTransferComponent
{
    public IComponent Component { get; set; }

    int Str;
    int Agi;
    int Wis;
    int Con;
    int Int;
    int Cha;

    int AttributePoints = 0;

    public void CreateComponent(string data)
    {
        string[] statsParse = data.Split(',');
        foreach(var stat in statsParse)
        {
            string[] statValue = stat.Split('=');
            switch(statValue[0])
            {
                case nameof(Str):
                    Str = int.Parse(statValue[1]);
                    break;
                case nameof(Agi):
                    Agi = int.Parse(statValue[1]);
                    break;
                case nameof(Wis):
                    Wis = int.Parse(statValue[1]);
                    break;
                case nameof(Con):
                    Con = int.Parse(statValue[1]);
                    break;
                case nameof(Int):
                    Int = int.Parse(statValue[1]);
                    break;
                case nameof(Cha):
                    Cha = int.Parse(statValue[1]);
                    break;
                case nameof(AttributePoints):
                    AttributePoints = int.Parse(statValue[1]);
                    break;
            }
        }
        Component = new Stats(Str, Agi, Con, Wis, Int, Cha, AttributePoints);
    }

    public string CreateSerializableData(IComponent component)
    {
        Stats stats = (Stats)component;
        return $"{nameof(Stats)}: Str={stats.Str}, Agi={stats.Agi}, Con={stats.Con}, Wis={stats.Wis}, Int={stats.Int}, Cha={stats.Cha}, AttributePoints={stats.AttributePoints}";
    }
}
