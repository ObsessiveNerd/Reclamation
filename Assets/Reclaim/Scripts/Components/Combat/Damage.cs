using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public int DamageAmount;
    public DamageType DamageType;

    public Damage(int amount, DamageType type)
    {
        DamageAmount = amount;
        DamageType = type;
    }
}

public static class Extensions
{
    public static bool GetMeleeDamageType(this List<Damage> list, out int meleeTypeIndex)
    {
        meleeTypeIndex = -1;
        for(int i = 0; i < list.Count; i++)
        {
            DamageType dType = list[i].DamageType;
            if (dType == DamageType.Blunt || dType == DamageType.Slashing || dType == DamageType.Piercing)
            {
                meleeTypeIndex = i;
                return true;
            }
        }
        return false;
    }
}
