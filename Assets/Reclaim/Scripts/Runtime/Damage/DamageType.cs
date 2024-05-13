using System;

public enum DamageType
{
    None,
    Slashing,
    Piercing,
    Blunt,
    Fire,
    Ice,
    Electric,
    Dark,
    Earth,
    Light,
    Arcane,
    Wind,
    Poison
}

[Serializable]
public class Damage
{
    public Dice DamageAmount;
    public DamageType DamageType;
}
