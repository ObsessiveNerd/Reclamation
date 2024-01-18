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
public class Stats : EntityComponentBehavior
{
    public int Str;
    public int Agi;
    public int Con;
    public int Wis;
    public int Int;
    public int Cha;

    public Stat PrimaryStatType;

    public int AttributePoints;

    public void Start()
    {
        //RegisteredEvents.Add(GameEventId.RollToHit);
        //RegisteredEvents.Add(GameEventId.GetStat);
        //RegisteredEvents.Add(GameEventId.GetStatRaw);
        //RegisteredEvents.Add(GameEventId.LevelUp);
        //RegisteredEvents.Add(GameEventId.BoostStat);
        //RegisteredEvents.Add(GameEventId.GetAttributePoints);
        //RegisteredEvents.Add(GameEventId.GetSpellSaveDC);
        //RegisteredEvents.Add(GameEventId.SavingThrow);
        //RegisteredEvents.Add(GameEventId.GetPrimaryStatType);
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

    int GetModifier(Stat s)
    {
        int stat = GetStat(s);
        return CalculateModifier(stat);
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
