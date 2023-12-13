using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CombatUtility
{
    static int CombatRatingBuffer => 4;

    static void CastSpell(GameObject source, GameObject target, GameObject weapon)
    {
        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
            .With(EventParameter.Value, 0);
        int mana = source.FireEvent(getMana).GetValue<int>(EventParameter.Value);

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
            .With(EventParameter.SpellList, new HashSet<string>());

        GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
            .With(EventParameter.Value, 0);
        int cost = weapon.FireEvent(manaCost).GetValue<int>(EventParameter.Value);
        if (cost <= mana)
        {
            if(!Services.PlayerManagerService.IsPlayableCharacter(source.ID))
            {
                GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile)
                    .With(EventParameter.Source, source.ID)
                    .With(EventParameter.InputDirection, PathfindingUtility.GetDirectionTo(source, target))
                    .With(EventParameter.TilePosition, Services.EntityMapService.GetPointWhereEntityIs(target));
                weapon.FireEvent(selectTile);
                selectTile.Release();
            }
            CastSpellAtTarget(weapon, source, target);

            GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                .With(EventParameter.Effect,
                    new Action<GameObject>((t) =>
                    {
                        CastSpellAtTarget(weapon, source, t);
                    }));
            weapon.FireEvent(affectArea);
            GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameter.Mana, cost);
            source.FireEvent(depleteMana);

            GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy).With(EventParameter.Value, 1.0f);
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

    public static void Attack(GameObject source, GameObject target, GameObject weapon, AttackType attackType = AttackType.None)
    {
        if(attackType == AttackType.None)
            attackType = GetWeaponTypeList(weapon)[0];

        RecLog.Log($"{source.Name} is performing a {attackType} attack with {weapon.Name}");

        if (attackType == AttackType.RangedSpell)
            CastRangedSpell(source, target, weapon);

        else if (attackType == AttackType.Spell)
            CastSpell(source, target, weapon);

        else if (attackType == AttackType.Ranged)
        {
            GameEvent getAmmo = GameEventPool.Get(GameEventId.GetAmmo)
                    .With(EventParameter.Value, null);

            GameObject ammo = EntityQuery.GetEntity(weapon.FireEvent(getAmmo).GetValue<string>(EventParameter.Value));
            if (ammo != null)
                weapon = ammo;
            getAmmo.Release();
            PhysicalAttack(source, target, ammo, AttackType.Finesse);
        }

        else
            PhysicalAttack(source, target, weapon, attackType);

        if (TypeWeaponUsesMana(attackType))
        {
            GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                    .With(EventParameter.Value, 0);
            int mana = source.FireEvent(getMana).GetValue<int>(EventParameter.Value);
            getMana.Release();

            GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
                    .With(EventParameter.Value, 0);
            int cost = weapon.FireEvent(manaCost).GetValue<int>(EventParameter.Value);
            manaCost.Release();

            if (cost > mana)
            {
                RecLog.Log($"{source.Name} does not have enough mana to cast {weapon.Name}");
                return;
            }

            GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameter.Mana, cost);
            source.FireEvent(depleteMana);

        }
        

        if (attackType != AttackType.Melee && attackType != AttackType.Finesse)
        {
            
            GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                    .With(EventParameter.SoundSource, source.ID)
                                    .With(EventParameter.Key, SoundKey.RangedAttack);
            weapon.FireEvent(playSound).Release();
        }

        

        //if(useEnergy)
        source.FireEvent(source, GameEventPool.Get(GameEventId.UseEnergy)
            .With(EventParameter.Value, 1f)).Release(); //todo: temp energy value.  Energy value should come from the weapon probably


    }

    static void CastRangedSpell(GameObject source, GameObject target, GameObject spell)
    {
        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                    .With(EventParameter.Value, 0);
        int mana = source.FireEvent(getMana).GetValue<int>(EventParameter.Value);
        getMana.Release();

        GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
                .With(EventParameter.Value, 0);
        int cost = spell.FireEvent(manaCost).GetValue<int>(EventParameter.Value);
        manaCost.Release();

        if (cost > mana)
        {
            RecLog.Log($"{source.Name} does not have enough mana to cast {spell.Name}");
            return;
        }

        SpellType spellType = GetSpellType(spell);

        GameEvent rollToHitEvent = GameEventPool.Get(GameEventId.RollToHit)
                .With(EventParameter.RollToHit, 0)
                .With(EventParameter.Crit, false)
                .With(EventParameter.SpellType, spellType)
                .With(EventParameter.WeaponType, AttackType.RangedSpell);
        int rollToHit = (int)source.FireEvent(source, rollToHitEvent).Paramters[EventParameter.RollToHit];

        RecLog.Log($"Roll to hit was {rollToHit}");

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
                .With(EventParameter.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(spell, amAttacking);
        checkWeaponAttack.Paramters.Add(EventParameter.DamageSource, source.ID);

        GameEvent attack = GameEventPool.Get(GameEventId.TakeDamage)
                .With(checkWeaponAttack.Paramters)
                .With(EventParameter.Attack, spell.ID)
                .With(EventParameter.WeaponType, AttackType.RangedSpell)
                .With(EventParameter.RollToHit, rollToHit)
                .With(EventParameter.Crit, rollToHitEvent.Paramters[EventParameter.Crit]);

        source.FireEvent(target, attack);

        GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                .With(EventParameter.Effect,
                    new Action<GameObject>((t) => t.FireEvent(attack)));

        GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameter.Mana, cost);
        source.FireEvent(depleteMana).Release();

        rollToHitEvent.Release();
        spell.FireEvent(affectArea);
        affectArea.Release();
        attack.Release();
        amAttacking.Release();
    }

    static void PhysicalAttack(GameObject source, GameObject target, GameObject weapon, AttackType attackType)
    {
        GameEvent rollToHitEvent = GameEventPool.Get(GameEventId.RollToHit)
                .With(EventParameter.RollToHit, 0)
                .With(EventParameter.Crit, false)
                .With(EventParameter.WeaponType, attackType);
        int rollToHit = (int)source.FireEvent(source, rollToHitEvent).Paramters[EventParameter.RollToHit];

        RecLog.Log($"Roll to hit was {rollToHit}");

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
                .With(EventParameter.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(weapon, amAttacking);
        checkWeaponAttack.Paramters.Add(EventParameter.DamageSource, source.ID);

        GameEvent getStatModForDamage = GameEventPool.Get(GameEventId.GetStat)
                .With(EventParameter.Value, 0);

        var damageList = checkWeaponAttack.GetValue<List<Damage>>(EventParameter.DamageList);
        int statBasedDamage = 0;
        bool addStatBasedDamage = false;
        switch (attackType)
        {
            case AttackType.Melee:
                getStatModForDamage = getStatModForDamage.With(EventParameter.StatType, Stat.Str);
                addStatBasedDamage = true;
                break;
            case AttackType.Finesse:
            case AttackType.Ranged:
                getStatModForDamage = getStatModForDamage.With(EventParameter.StatType, Stat.Agi);
                addStatBasedDamage = true;
                break;
        }

        if (addStatBasedDamage)
        {
            statBasedDamage = source.FireEvent(getStatModForDamage).GetValue<int>(EventParameter.Value);
            if(damageList.GetMeleeDamageType(out int meleeIndex)) damageList[meleeIndex].DamageAmount += statBasedDamage;
        }

        GameEvent attack = GameEventPool.Get(GameEventId.TakeDamage)
                .With(checkWeaponAttack.Paramters)
                .With(EventParameter.Attack, weapon.ID)
                .With(EventParameter.WeaponType, attackType)
                .With(EventParameter.RollToHit, rollToHit)
                .With(EventParameter.Crit, rollToHitEvent.Paramters[EventParameter.Crit]);

        source.FireEvent(target, attack);

        GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                .With(EventParameter.Effect,
                    new Action<GameObject>((t) => t.FireEvent(attack)));

        rollToHitEvent.Release();
        weapon.FireEvent(affectArea);
        affectArea.Release();
        attack.Release();
        getStatModForDamage.Release();
        amAttacking.Release();
    }

    static void CastSpellAtTarget(GameObject weapon, GameObject source, GameObject target)
    {
        string targetId = target.ID;

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
            .With(EventParameter.Entity, source.ID)
            .With(EventParameter.Target, targetId)
            .With(EventParameter.DamageList, new List<Damage>());
        weapon.FireEvent(amAttacking);

        target = Services.EntityMapService.GetEntity(amAttacking.GetValue<string>(EventParameter.Target));

        GameEvent castSpell = GameEventPool.Get(GameEventId.TakeDamage)
            .With(EventParameter.Attack, weapon.ID)
            .With(EventParameter.DamageSource, source.ID)
            .With(amAttacking.Paramters);

        GameEvent spellCast = GameEventPool.Get(GameEventId.CastSpellEffect)
                                .With(EventParameter.Entity, source.ID)
                                .With(EventParameter.Target, target.ID);
        weapon.FireEvent(spellCast);
        spellCast.Release();

        GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                .With(EventParameter.SoundSource, source.ID)
                                .With(EventParameter.Key, SoundKey.Cast);
        weapon.FireEvent(playSound).Release();

        target.FireEvent(castSpell);

        if(!weapon.HasComponent(typeof(Summon)))
        {
            var sourceGo = WorldUtility.GetGameObject(source);
            var targetGo = WorldUtility.GetGameObject(target);
            if(sourceGo != null && targetGo != null)
            {
                GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                   .With(EventParameter.Entity, sourceGo.transform.position)
                    .With(EventParameter.Target, targetGo.transform.position);
                weapon.FireEvent(fireRangedWeapon).Release();
            }
        }
        amAttacking.Release();
        castSpell.Release();
    }

    public static List<GameObject> GetSpells(GameObject source)
    {
        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                        .With(EventParameter.SpellList, new HashSet<string>());

        HashSet<string> spells = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameter.SpellList);
        getSpells.Release();

        return spells.Select(id => Services.EntityMapService.GetEntity(id)).ToList();
    }

    public static List<AttackType> GetWeaponTypeList(GameObject weapon)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetWeaponType)
            .With(EventParameter.WeaponType, new List<AttackType>());

        weapon.FireEvent(weapon, typedEvent);
        var retValue = typedEvent.GetValue<List<AttackType>>(EventParameter.WeaponType);
        typedEvent.Release();
        return retValue;
    }

    public static SpellType GetSpellType(GameObject spell)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetSpellType)
            .With(EventParameter.SpellType, SpellType.None);

        spell.FireEvent(typedEvent);
        var retValue = typedEvent.GetValue<SpellType>(EventParameter.SpellType);
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
        if (weaponType == AttackType.Spell || weaponType == AttackType.RangedSpell)
            return true;
        return false;
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
