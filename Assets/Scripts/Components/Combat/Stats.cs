using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : Component
{
    private int m_Str;
    private int m_Agi;
    private int m_Con;
    private int m_Wis;
    private int m_Int;
    private int m_Cha;

    public Stats(int Str, int Agi, int Con, int Wis, int Int, int Cha)
    {
        SetStats(Str, Agi, Con, Wis, Int, Cha);
        RegisteredEvents.Add(GameEventId.RollToHit);
    }

    void SetStats(int Str, int Agi, int Con, int Wis, int Int, int Cha)
    {
        m_Str = Str;
        m_Agi = Agi;
        m_Con = Con;
        m_Wis = Wis;
        m_Int = Int;
        m_Cha = Cha;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        int totalRoll = Dice.Roll("1d20");
        TypeWeapon weaponType = (TypeWeapon)gameEvent.Paramters[EventParameters.WeaponType];
        switch(weaponType)
        {
            case TypeWeapon.Melee:
            case TypeWeapon.StrSpell:
                totalRoll += GetModifier(m_Str);
                break;
            case TypeWeapon.Finesse:
            case TypeWeapon.Ranged:
            case TypeWeapon.AgiSpell:
                totalRoll += GetModifier(m_Agi);
                break;
            case TypeWeapon.MagicStaff:
            case TypeWeapon.IntSpell:
                totalRoll += GetModifier(m_Int);
                break;
            case TypeWeapon.ConSpell:
                totalRoll += GetModifier(m_Con);
                break;
            case TypeWeapon.WisSpell:
                totalRoll += GetModifier(m_Wis);
                break;
            case TypeWeapon.ChaSpell:
                totalRoll += GetModifier(m_Cha);
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
}
