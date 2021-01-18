using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : Component
{
    public int Str;
    public int Agi;
    public int Con;
    public int Wis;
    public int Int;
    public int Cha;

    public Stats(int Str, int Agi, int Con, int Wis, int Int, int Cha)
    {
        SetStats(Str, Agi, Con, Wis, Int, Cha);
        RegisteredEvents.Add(GameEventId.RollToHit);
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
            }
        }
        Component = new Stats(Str, Agi, Con, Wis, Int, Cha);
    }

    public string CreateSerializableData(IComponent component)
    {
        Stats stats = (Stats)component;
        return $"{nameof(Stats)}: Str={stats.Str}, Agi={stats.Agi}, Con={stats.Con}, Wis={stats.Wis}, Int={stats.Int}, Cha={stats.Cha}";
    }
}
