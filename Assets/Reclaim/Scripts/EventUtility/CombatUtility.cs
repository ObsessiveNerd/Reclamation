﻿using System;
using System.Collections.Generic;
using System.Linq;

public static class CombatUtility
{
    static int CombatRatingBuffer => 4;

    public static void CastSpell(IEntity source, IEntity target, IEntity weapon)
    {
        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
            .With(EventParameters.Value, 0);
        int mana = source.FireEvent(getMana).GetValue<int>(EventParameters.Value);

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
            .With(EventParameters.SpellList, new HashSet<string>());

        GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
            .With(EventParameters.Value, 0);
        int cost = weapon.FireEvent(manaCost).GetValue<int>(EventParameters.Value);
        if (cost <= mana)
        {
            CastSpellAtTarget(weapon, source, target);

            GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile)
            .With(EventParameters.TilePosition, Services.EntityMapService.GetPointWhereEntityIs(target));
            weapon.FireEvent(selectTile);
            selectTile.Release();

            GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                .With(EventParameters.Effect,
                    new Action<IEntity>((t) =>
                    {
                        CastSpellAtTarget(weapon, source, t);
                    }));
            weapon.FireEvent(affectArea);
            GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, cost);
            source.FireEvent(depleteMana);

            GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy).With(EventParameters.Value, 1.0f);
            source.FireEvent(useEnergy);

            useEnergy.Release();
            depleteMana.Release();
            affectArea.Release();
            manaCost.Release();
            getSpells.Release();
            getMana.Release();
        }

        manaCost.Release();
        getSpells.Release();
        getMana.Release();
    }

    public static void Attack(IEntity source, IEntity target, IEntity weapon, AttackType attackType)
    {
        if (attackType == AttackType.None)
            return;

        RecLog.Log($"{source.Name} is performing a {attackType} attack with {weapon.Name}");

        if (TypeWeaponUsesMana(attackType))
        {
            GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                    .With(EventParameters.Value, 0);
            int mana = source.FireEvent(getMana).GetValue<int>(EventParameters.Value);
            getMana.Release();

            GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
                    .With(EventParameters.Value, 0);
            int cost = weapon.FireEvent(manaCost).GetValue<int>(EventParameters.Value);
            manaCost.Release();

            if (cost > mana)
            {
                RecLog.Log($"{source.Name} does not have enough mana to cast {weapon.Name}");
                return;
            }

            GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, cost);
            source.FireEvent(depleteMana);

        }

        if (attackType == AttackType.Ranged)
        {
            GameEvent getAmmo = GameEventPool.Get(GameEventId.GetAmmo)
                    .With(EventParameters.Value, null);

            IEntity ammo = EntityQuery.GetEntity(weapon.FireEvent(getAmmo).GetValue<string>(EventParameters.Value));
            if (ammo != null)
                weapon = ammo;
            getAmmo.Release();
        }


        GameEvent rollToHitEvent = GameEventPool.Get(GameEventId.RollToHit)
                .With(EventParameters.RollToHit, 0)
                .With(EventParameters.Crit, false)
                .With(EventParameters.WeaponType, attackType);
        int rollToHit = (int)source.FireEvent(source, rollToHitEvent).Paramters[EventParameters.RollToHit];

        RecLog.Log($"Roll to hit was {rollToHit}");

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
                .With(EventParameters.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(weapon, amAttacking);
        checkWeaponAttack.Paramters.Add(EventParameters.DamageSource, source.ID);

        GameEvent getStatModForDamage = GameEventPool.Get(GameEventId.GetStat)
                .With(EventParameters.Value, 0);

        var damageList = checkWeaponAttack.GetValue<List<Damage>>(EventParameters.DamageList);
        int statBasedDamage = 0;
        bool addStatBasedDamage = false;
        switch (attackType)
        {
            case AttackType.Melee:
                getStatModForDamage = getStatModForDamage.With(EventParameters.StatType, Stat.Str);
                addStatBasedDamage = true;
                break;
            case AttackType.Finesse:
            case AttackType.Ranged:
                getStatModForDamage = getStatModForDamage.With(EventParameters.StatType, Stat.Agi);
                addStatBasedDamage = true;
                break;
        }

        if (addStatBasedDamage)
        {
            statBasedDamage = source.FireEvent(getStatModForDamage).GetValue<int>(EventParameters.Value);
            if(damageList.GetMeleeDamageType(out int meleeIndex)) damageList[meleeIndex].DamageAmount += statBasedDamage;
        }

        GameEvent attack = GameEventPool.Get(GameEventId.TakeDamage)
                .With(checkWeaponAttack.Paramters)
                .With(EventParameters.Attack, weapon.ID)
                .With(EventParameters.WeaponType, attackType)
                .With(EventParameters.RollToHit, rollToHit)
                .With(EventParameters.Crit, rollToHitEvent.Paramters[EventParameters.Crit]);

        source.FireEvent(target, attack);

        GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                .With(EventParameters.Effect,
                    new Action<IEntity>((t) => t.FireEvent(attack)));

        rollToHitEvent.Release();
        weapon.FireEvent(affectArea);
        affectArea.Release();
        attack.Release();

        if (attackType != AttackType.Melee && attackType != AttackType.Finesse)
        {
            if(!weapon.HasComponent(typeof(Summon)))
            {
                GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                    .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
                    .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                weapon.FireEvent(fireRangedWeapon).Release();
            }
            GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                    .With(EventParameters.SoundSource, source.ID)
                                    .With(EventParameters.Key, SoundKey.RangedAttack);
            weapon.FireEvent(playSound).Release();
        }

        getStatModForDamage.Release();
        amAttacking.Release();

        //if(useEnergy)
        source.FireEvent(source, GameEventPool.Get(GameEventId.UseEnergy)
            .With(EventParameters.Value, 1f)).Release(); //todo: temp energy value.  Energy value should come from the weapon probably
    }

    static void CastSpellAtTarget(IEntity weapon, IEntity source, IEntity target)
    {
        string targetId = target.ID;

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
            .With(EventParameters.Entity, source.ID)
            .With(EventParameters.Target, targetId)
            .With(EventParameters.DamageList, new List<Damage>());
        weapon.FireEvent(amAttacking);

        target = Services.EntityMapService.GetEntity(amAttacking.GetValue<string>(EventParameters.Target));

        GameEvent castSpell = GameEventPool.Get(GameEventId.TakeDamage)
            .With(EventParameters.Attack, weapon.ID)
            .With(EventParameters.DamageSource, source.ID)
            .With(amAttacking.Paramters);

        GameEvent spellCast = GameEventPool.Get(GameEventId.CastSpellEffect)
                                .With(EventParameters.Entity, source.ID)
                                .With(EventParameters.Target, target.ID);
        weapon.FireEvent(spellCast);
        spellCast.Release();

        GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                .With(EventParameters.SoundSource, source.ID)
                                .With(EventParameters.Key, SoundKey.Cast);
        weapon.FireEvent(playSound).Release();

        target.FireEvent(castSpell);

        if(!weapon.HasComponent(typeof(Summon)))
        {
            GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
            .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
            .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
            weapon.FireEvent(fireRangedWeapon).Release();
        }
        amAttacking.Release();
        castSpell.Release();
    }

    public static List<IEntity> GetSpells(IEntity source)
    {
        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                        .With(EventParameters.SpellList, new HashSet<string>());

        HashSet<string> spells = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameters.SpellList);
        getSpells.Release();

        return spells.Select(id => Services.EntityMapService.GetEntity(id)).ToList();
    }

    public static List<AttackType> GetWeaponTypeList(IEntity weapon)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetWeaponType)
            .With(EventParameters.WeaponType, new List<AttackType>());

        weapon.FireEvent(weapon, typedEvent);
        var retValue = typedEvent.GetValue<List<AttackType>>(EventParameters.WeaponType);
        typedEvent.Release();
        return retValue;
    }

    public static SpellType GetSpellType(IEntity spell)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetSpellType)
            .With(EventParameters.SpellType, SpellType.None);

        spell.FireEvent(typedEvent);
        var retValue = typedEvent.GetValue<SpellType>(EventParameters.SpellType);
        typedEvent.Release();
        return retValue;
    }

    public static bool TypeWeaponRequiresAttackRoll(AttackType weaponType)
    {
        if (weaponType == AttackType.RangedSpell || weaponType == AttackType.Finesse || weaponType == AttackType.Melee || weaponType == AttackType.Ranged)
            return true;
        return false;
    }

    public static bool TypeWeaponUsesMana(AttackType weaponType)
    {
        if (weaponType == AttackType.Finesse || weaponType == AttackType.Melee || weaponType == AttackType.Ranged)
            return false;
        return true;
    }

    public static bool ICanTakeThem(int sourceValue, int targetValue)
    {
        if (targetValue <= (sourceValue + CombatRatingBuffer))
            return true;
        return false;
    }

    public static bool AmIAfraid(int sourceCR, int targetCR)
    {
        if (targetCR > (sourceCR + CombatRatingBuffer))
            return true;
        return false;
    }
}
