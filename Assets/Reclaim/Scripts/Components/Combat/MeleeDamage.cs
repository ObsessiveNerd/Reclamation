using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : EntityComponent
{
    public Dice DamageAmount = new Dice("1d1");
    public DamageType DamageType = DamageType.None;
}