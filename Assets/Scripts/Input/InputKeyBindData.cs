﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestedAction
{
    MoveN,
    MoveNE,
    MoveE,
    MoveSE,
    MoveS,
    MoveSW,
    MoveW,
    MoveNW,
    OpenInventory,
    FireRangedWeapon,
    Look,
    CastSpell,
    PickupItem,
    RotateCharacter,
    SpellSelect1,
    SpellSelect2,
    SpellSelect3,
    SpellSelect4,
    SpellSelect5
}

[Serializable]
public class InputKeyBindData
{
    [SerializeField]
    KeyCode MoveN;
    [SerializeField]
    KeyCode MoveNE;
    [SerializeField]
    KeyCode MoveE;
    [SerializeField]
    KeyCode MoveSE;
    [SerializeField]
    KeyCode MoveS;
    [SerializeField]
    KeyCode MoveSW;
    [SerializeField]
    KeyCode MoveW;
    [SerializeField]
    KeyCode MoveNW;
    [SerializeField]
    KeyCode OpenInventory;
    [SerializeField]
    KeyCode FireRangedWeapon;
    [SerializeField]
    KeyCode Look;
    [SerializeField]
    KeyCode CastSpell;
    [SerializeField]
    KeyCode PickupItem;
    [SerializeField]
    KeyCode RotateCharacter;
    [SerializeField]
    KeyCode SpellSelect1;
    [SerializeField]
    KeyCode SpellSelect2;
    [SerializeField]
    KeyCode SpellSelect3;
    [SerializeField]
    KeyCode SpellSelect4;
    [SerializeField]
    KeyCode SpellSelect5;

    public InputKeyBindData()
    {
        MoveN = KeyCode.Keypad8;
        MoveNE = KeyCode.Keypad9;
        MoveE = KeyCode.Keypad6;
        MoveSE = KeyCode.Keypad3;
        MoveS = KeyCode.Keypad2;
        MoveSW = KeyCode.Keypad1;
        MoveW = KeyCode.Keypad4;
        MoveNW = KeyCode.Keypad7;

        OpenInventory = KeyCode.I;
        FireRangedWeapon = KeyCode.F;
        Look = KeyCode.L;
        CastSpell = KeyCode.C;
        PickupItem = KeyCode.Space;

        RotateCharacter = KeyCode.Tab;
        SpellSelect1 = KeyCode.Alpha1;
        SpellSelect2 = KeyCode.Alpha2;
        SpellSelect3 = KeyCode.Alpha3;
        SpellSelect4 = KeyCode.Alpha4;
        SpellSelect5 = KeyCode.Alpha5;
    }
}
