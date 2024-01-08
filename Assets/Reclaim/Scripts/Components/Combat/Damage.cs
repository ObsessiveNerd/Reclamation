using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Damage
{
    public int RollToHit;
    public int DamageAmount;
    public DamageType Type;
}
