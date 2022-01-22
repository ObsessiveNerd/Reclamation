﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : Component
{
    const int kBaseAC = 10;
    public override int Priority => 1;

    public Defense()
    {
        RegisteredEvents.Add(GameEventId.TakeDamage);
        //RegisteredEvents.Add(GameEventId.Sharpness);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            TypeWeapon weaponType = gameEvent.GetValue<TypeWeapon>(EventParameters.WeaponType);
            if (weaponType == TypeWeapon.Melee 
                || weaponType == TypeWeapon.Finesse 
                || weaponType == TypeWeapon.Ranged 
                || weaponType == TypeWeapon.RangedSpell)
            {
                int rollToHit = (int)gameEvent.Paramters[EventParameters.RollToHit];
                GameEvent getArmor = GameEventPool.Get(GameEventId.AddArmorValue)
                    .With(EventParameters.Value, 0);
                int armorBonus = (int)FireEvent(Self, getArmor).Paramters[EventParameters.Value];
                getArmor.Release();
                if (rollToHit >= kBaseAC + armorBonus)
                    RecLog.Log($"{Self.Name} was hit!");
                else
                {
                    RecLog.Log($"Attack missed because armor was {kBaseAC + armorBonus}!");
                    gameEvent.ContinueProcessing = false;
                }
            }
            else
            {
                IEntity weapon = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Weapon));
                IEntity damageSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.DamageSource));
                GameEvent getSpellSaveDC = GameEventPool.Get(GameEventId.GetSpellSaveDC)
                    .With(EventParameters.Value, -1);
                int saveDC = damageSource.FireEvent(getSpellSaveDC).GetValue<int>(EventParameters.Value);
                getSpellSaveDC.Release();

                GameEvent save = GameEventPool.Get(GameEventId.SavingThrow)
                    .With(EventParameters.Value, 0)
                    .With(EventParameters.WeaponType, weaponType);
                FireEvent(Self, save);
                int saveThrow = save.GetValue<int>(EventParameters.Value);
                save.Release();

                Debug.Log($"{weapon.Name} Spell was cast, Save DC was {saveDC} and save was {saveThrow}");

                if (saveThrow < saveDC)
                {
                    GameEvent saveFailed = GameEventPool.Get(GameEventId.SaveFailed)
                        .With(EventParameters.SpellContinues, true)
                        .With(EventParameters.DamageList, gameEvent.Paramters[EventParameters.DamageList]);
                    FireEvent(weapon, saveFailed);
                    if (!saveFailed.GetValue<bool>(EventParameters.SpellContinues))
                        gameEvent.ContinueProcessing = false;
                    
                    saveFailed.Release();
                }
            }
        }

        //if (gameEvent.ID == GameEventId.Sharpness)
        //{
        //    int rollToHit = (int)gameEvent.Paramters[EventParameters.RollToHit];
        //    GameEvent getArmor = GameEventPool.Get(GameEventId.AddArmorValue)
        //        .With(EventParameters.Value, 0);
        //    int armorBonus = (int)FireEvent(Self, getArmor).Paramters[EventParameters.Value];
        //    getArmor.Release();

        //    if (rollToHit < kBaseAC + armorBonus)
        //        RecLog.Log("Nothing was severed.");
        //    else
        //        FireEvent(Self, GameEventPool.Get(GameEventId.SeverBodyPart));
        //}
    }
}

public class DTO_Defense : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Defense();
    }

    public string CreateSerializableData(IComponent comp)
    {
        return nameof(Defense);
    }
}
