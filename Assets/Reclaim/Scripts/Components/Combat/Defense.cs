using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        GameObject weapon = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Attack));
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            AttackType weaponType = CombatUtility.GetWeaponTypeList(weapon).FirstOrDefault(); //gameEvent.GetValue<AttackType>(EventParameters.WeaponType);
            if (weaponType == AttackType.Melee
                || weaponType == AttackType.Finesse
                || weaponType == AttackType.Ranged 
                || weaponType == AttackType.RangedSpell)
            {
                int rollToHit = (int)gameEvent.Paramters[EventParameter.RollToHit];
                GameEvent getArmor = GameEventPool.Get(GameEventId.AddArmorValue)
                    .With(EventParameter.Value, 0);
                int armorBonus = (int)FireEvent(Self, getArmor).Paramters[EventParameter.Value];
                getArmor.Release();
                if (rollToHit >= kBaseAC + armorBonus)
                    RecLog.Log($"{Self.Name} was hit!");
                else
                {
                    GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                            .With(EventParameter.SoundSource, Self.ID)
                                            .With(EventParameter.Key, SoundKey.AttackMiss);
                    EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Attack)).FireEvent(playSound);
                    playSound.Release();

                    RecLog.Log($"Attack missed because armor was {kBaseAC + armorBonus}!");
                    gameEvent.ContinueProcessing = false;
                }
            }
            else
            {
                if(!weapon.HasComponent(typeof(SelfTargetingSpell)))
                {
                    GameObject damageSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.DamageSource));
                    GameEvent getSpellSaveDC = GameEventPool.Get(GameEventId.GetSpellSaveDC)
                    .With(EventParameter.SpellType, CombatUtility.GetSpellType(weapon))
                    .With(EventParameter.Value, -1);
                    int saveDC = damageSource.FireEvent(getSpellSaveDC).GetValue<int>(EventParameter.Value);
                    getSpellSaveDC.Release();

                    GameEvent save = GameEventPool.Get(GameEventId.SavingThrow)
                    .With(EventParameter.Value, 0)
                    .With(EventParameter.WeaponType, weaponType);
                    FireEvent(Self, save);
                    int saveThrow = save.GetValue<int>(EventParameter.Value);
                    save.Release();

                    Debug.Log($"{weapon.Name} Spell was cast, Save DC was {saveDC} and save was {saveThrow}");

                    if (saveThrow < saveDC)
                    {
                        GameEvent saveFailed = GameEventPool.Get(GameEventId.SaveFailed)
                        .With(EventParameter.SpellContinues, true)
                        .With(EventParameter.DamageList, gameEvent.Paramters[EventParameter.DamageList]);
                        FireEvent(weapon, saveFailed);
                        if (!saveFailed.GetValue<bool>(EventParameter.SpellContinues))
                            gameEvent.ContinueProcessing = false;

                        saveFailed.Release();
                    }
                }
            }
        }
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
