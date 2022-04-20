using System;
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

[Serializable]
public class Stats : EntityComponent
{
    public override int Priority => 3;

    public int Str;
    public int Agi;
    public int Con;
    public int Wis;
    public int Int;
    public int Cha;

    public Stat PrimaryStatType;

    public int AttributePoints;

    public Stats(int Str, int Agi, int Con, int Wis, int Int, int Cha, int attributePoints, Stat primaryStatType)
    {
        SetStats(Str, Agi, Con, Wis, Int, Cha);
        AttributePoints = attributePoints;
        PrimaryStatType = primaryStatType;

        RegisteredEvents.Add(GameEventId.RollToHit);
        RegisteredEvents.Add(GameEventId.GetStat);
        RegisteredEvents.Add(GameEventId.GetStatRaw);
        RegisteredEvents.Add(GameEventId.LevelUp);
        RegisteredEvents.Add(GameEventId.BoostStat);
        RegisteredEvents.Add(GameEventId.GetAttributePoints);
        RegisteredEvents.Add(GameEventId.GetSpellSaveDC);
        RegisteredEvents.Add(GameEventId.SavingThrow);
        RegisteredEvents.Add(GameEventId.GetPrimaryStatType);
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
            gameEvent.Paramters[EventParameters.Crit] = ((totalRoll + CalculateModifier(Wis)) >= 20);
            AttackType weaponType = (AttackType)gameEvent.Paramters[EventParameters.WeaponType];
            totalRoll += CalculateModifier(weaponType);
            gameEvent.Paramters[EventParameters.RollToHit] = totalRoll;
        }
        else if(gameEvent.ID == GameEventId.GetStat)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            gameEvent.Paramters[EventParameters.Value] = GetModifier(s);
        }
        else if(gameEvent.ID == GameEventId.GetStatRaw)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            gameEvent.Paramters[EventParameters.Value] = GetStat(s);
        }
        else if(gameEvent.ID == GameEventId.LevelUp)
        {
            //int newLevel = gameEvent.GetValue<int>(EventParameters.Level);
            //if(newLevel % 2 == 0)
                AttributePoints += 4;
        }
        else if (gameEvent.ID == GameEventId.GetSpellSaveDC)
        {
            SpellType spellType = gameEvent.GetValue<SpellType>(EventParameters.SpellType);
            gameEvent.Paramters[EventParameters.Value] = 11 + GetModifier(GetStatTypeForSpell(spellType));
        }
        else if (gameEvent.ID == GameEventId.SavingThrow)
        {
            int roll = Dice.Roll("1d20");
            var spellType = gameEvent.GetValue<SpellType>(EventParameters.WeaponType);
            roll += CalculateModifier(spellType);
            if (GetStatTypeForSpell(spellType) == PrimaryStatType)
                roll += 2;
            gameEvent.Paramters[EventParameters.Value] = roll;

        }
        else if(gameEvent.ID == GameEventId.BoostStat)
        {
            Stat s = gameEvent.GetValue<Stat>(EventParameters.StatType);
            int amountToBoost = gameEvent.GetValue<int>(EventParameters.StatBoostAmount);
            bool costAttributePoint = gameEvent.GetValue<bool>(EventParameters.Cost);

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

            GameEvent statBoosted = GameEventPool.Get(GameEventId.StatBoosted)
                .With(EventParameters.Stats, this);

            FireEvent(Self, statBoosted, true).Release();

            if(costAttributePoint)
                AttributePoints = Mathf.Max(0, AttributePoints - 1);
        }
        else if(gameEvent.ID == GameEventId.GetAttributePoints)
        {
            gameEvent.Paramters[EventParameters.AttributePoints] = AttributePoints;
        }
        if(gameEvent.ID == GameEventId.GetPrimaryStatType)
        {
            gameEvent.Paramters[EventParameters.Value] = PrimaryStatType;
        }
    }

    int GetModifier(Stat s)
    {
        int stat = GetStat(s);
        return CalculateModifier(stat);
    }

    Stat GetStatTypeForWeapon(AttackType weaponType)
    {
        Stat value = 0;
        switch(weaponType)
        {
            case AttackType.Melee:
                value = Stat.Str;
                break;
            case AttackType.Finesse:
            case AttackType.Ranged:
                value = Stat.Agi;
                break;
            case AttackType.RangedSpell:
                value = PrimaryStatType;
                break;
        }

        return value;
    }

    Stat GetStatTypeForSpell(SpellType spellType)
    {
        Stat value = 0;
        switch (spellType)
        {
            case SpellType.StrSpell:
                value = Stat.Str;
                break;
            case SpellType.AgiSpell:
                value = Stat.Agi;
                break;
            case SpellType.IntSpell:
                value = Stat.Int;
                break;
            case SpellType.ConSpell:
                value = Stat.Con;
                break;
            case SpellType.WisSpell:
                value = Stat.Wis;
                break;
            case SpellType.ChaSpell:
                value = Stat.Cha;
                break;
        }

        return value;
    }

    int CalculateModifier(SpellType spellType)
    {
        int value = 0;
        switch (spellType)
        {
            case SpellType.StrSpell:
                value = CalculateModifier(Str);
                break;
            case SpellType.AgiSpell:
                value = CalculateModifier(Agi);
                break;
            case SpellType.IntSpell:
                value = CalculateModifier(Int);
                break;
            case SpellType.ConSpell:
                value = CalculateModifier(Con);
                break;
            case SpellType.WisSpell:
                value = CalculateModifier(Wis);
                break;
            case SpellType.ChaSpell:
                value = CalculateModifier(Cha);
                break;
        }

        return value;
    }

    int CalculateModifier(AttackType weaponType)
    {
        int value = 0;
        switch(weaponType)
        {
            case AttackType.Melee:
                value = CalculateModifier(Str);
                break;
            case AttackType.Finesse:
            case AttackType.Ranged:
                value = CalculateModifier(Agi);
                break;
            case AttackType.RangedSpell:
                value = GetModifier(PrimaryStatType);
                break;
        }

        return value;
    }

    int GetStat(Stat statType)
    {
        int value = 0;
        switch (statType)
        {
            case Stat.Str:
                value = Str;
                break;
            case Stat.Agi:
                value = Agi;
                break;
            case Stat.Con:
                value = Con;
                break;
            case Stat.Wis:
                value = Wis;
                break;
            case Stat.Int:
                value = Int;
                break;
            case Stat.Cha:
                value = Cha;
                break;
        }

        return value;
    }

    public int CalculateModifier(int value)
    {
        return (value - 10) / 2;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
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
    Stat PrimaryStatType;

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
                case nameof(PrimaryStatType):
                    PrimaryStatType = (Stat)Enum.Parse(typeof(Stat), statValue[1]);
                    break;
            }
        }
        Component = new Stats(Str, Agi, Con, Wis, Int, Cha, AttributePoints, PrimaryStatType);
    }

    public string CreateSerializableData(IComponent component)
    {
        Stats stats = (Stats)component;
        return $"{nameof(Stats)}: Str={stats.Str}, Agi={stats.Agi}, Con={stats.Con}, Wis={stats.Wis}, Int={stats.Int}, Cha={stats.Cha}, AttributePoints={stats.AttributePoints}, {nameof(stats.PrimaryStatType)}={stats.PrimaryStatType}";
    }
}
