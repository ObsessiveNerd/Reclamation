using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Damage : EntityComponent
{
    public Dice DamageAmount;
    public DamageType Type;
}
