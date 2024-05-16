using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effects
{
    public string Name;
    public abstract string Description { get; }
    public virtual void OnEquip(GameObject target) { }
    public virtual void OnUnequip(GameObject source) { }
    public virtual void OnUse(GameObject target) { }
    public virtual void OnHit(GameObject target) { }
}

public enum HealableValues
{
    Health,
    Mana
}

[Serializable]
public class Heal : Effects
{
    public HealableValues ValueToHeal;
    public Dice HealAmount;

    public override string Description
    {
        get
        {
            return "";
            //string description = $"Increases {ValueToHeal} by {HealAmount.m_AmountOfDice}d{HealAmount.m_DAmount}";
            //if (HealAmount.m_Modifiers > 0)
            //    description += $"+{HealAmount.m_Modifiers}";
            //return description; 
        }
    }

    public override void OnUse(GameObject target)
    {
        int heal = HealAmount.Roll();
        switch (ValueToHeal)
        {
            case HealableValues.Health:
                var health = target.GetComponent<Health>();
                if (health != null)
                    health.Heal(heal);
                break;

            case HealableValues.Mana:

                break;
        }
    }
}

[Serializable]
public class PermanentlyIncreaseValue : Effects
{
    public HealableValues ValueToIncease;
    public int Amount;

    public override string Description
    {
        get
        {
            string description = $"Permanently increases {ValueToIncease} by {Amount}";
            return description;
        }
    }
}
