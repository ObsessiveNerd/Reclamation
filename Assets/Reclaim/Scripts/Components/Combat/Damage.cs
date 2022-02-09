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
