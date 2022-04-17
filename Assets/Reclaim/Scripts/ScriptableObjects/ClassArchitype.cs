using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassArchitype.asset", menuName = "Reclamation/Create Class")]
public class ClassArchitype : ScriptableObject
{
    public string Name;

    public Stat PrimaryStatType;

    public int Str;
    public int Agi;
    public int Con;
    public int Wis;
    public int Int;
    public int Cha;

    public string HeadEquip;
    public string ArmEquip1;
    public string ArmEquip2;
    public string TorosEquip;
    public string LegEquip1;
    public string BackEquip;
    public string NeckEquip;
    public string Finger1Equip;
    public string Finger2Equip;

    List<string> m_Equipment
    {
        get
        {
            return new List<string>()
            {
                HeadEquip,
                ArmEquip1,
                ArmEquip2,
                TorosEquip,
                LegEquip1,
                BackEquip,
                NeckEquip,
                Finger1Equip,
                Finger2Equip
            };
        }
    }

    public string GetReadout()
    {
        StringBuilder sb = new StringBuilder();
        //sb.AppendLine(Name);
        ////sb.AppendLine("----------------");

        sb.AppendLine($"Strength: {Str}");
        sb.AppendLine($"Agility: {Agi}");
        sb.AppendLine($"Constituion: {Con}");
        sb.AppendLine($"Intelligence: {Int}");
        sb.AppendLine($"Wisdom: {Wis}");
        sb.AppendLine($"Charisma: {Cha}");
        sb.AppendLine("----------------");

        sb.AppendLine("Starting Equipment");
        foreach (var equipment in m_Equipment)
        {
            if (!string.IsNullOrEmpty(equipment))
                sb.Append(equipment + ",");
        }

        return sb.ToString().TrimEnd(',');
    }
}
