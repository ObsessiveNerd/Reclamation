using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : EntityComponent
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
            AttackType weaponType = gameEvent.GetValue<AttackType>(EventParameters.WeaponType);
            if (weaponType == AttackType.Melee 
                || weaponType == AttackType.Finesse 
                || weaponType == AttackType.Ranged 
                || weaponType == AttackType.RangedSpell)
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
                    GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                            .With(EventParameters.Key, SoundKeys.AttackMiss);
                    gameEvent.GetValue<IEntity>(EventParameters.Attack).FireEvent(playSound);
                    playSound.Release();

                    RecLog.Log($"Attack missed because armor was {kBaseAC + armorBonus}!");
                    gameEvent.ContinueProcessing = false;
                }
            }
            else
            {
                IEntity weapon = gameEvent.GetValue<IEntity>(EventParameters.Attack);
                IEntity damageSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.DamageSource));
                GameEvent getSpellSaveDC = GameEventPool.Get(GameEventId.GetSpellSaveDC)
                    .With(EventParameters.SpellType, CombatUtility.GetSpellType(weapon))
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
