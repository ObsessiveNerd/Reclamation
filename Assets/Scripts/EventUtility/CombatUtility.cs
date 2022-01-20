using System;
using System.Collections.Generic;
using System.Linq;

public static class CombatUtility
{
    static int CombatRatingBuffer => 4;

    public static void Attack(IEntity source, IEntity target, IEntity weapon, bool isMelee, bool useEnergy = true)
    {
        var targetPos = WorldUtility.GetEntityPosition(target);
        if (targetPos == null || targetPos == Point.InvalidPoint || targetPos == new Point(0, 0))
            return;

        TypeWeapon weaponType = GetWeaponTypes(weapon).FirstOrDefault((t) =>
        {
            if(isMelee)
            {
                if(t == TypeWeapon.Melee || t == TypeWeapon.Finesse)
                    return true;
                return false;
            }
            else
            {
                if(t != TypeWeapon.Melee && t != TypeWeapon.Finesse)
                    return true;
                return false;
            }
        });
        RecLog.Log($"{source.Name} is performing a {weaponType} attack with {weapon.Name}");
        
        if (weaponType == TypeWeapon.None)
            return;

        GameEvent rollToHitEvent = GameEventPool.Get(GameEventId.RollToHit)
            .With(EventParameters.RollToHit, 0)
            .With(EventParameters.WeaponType, weaponType);
        int rollToHit = (int)source.FireEvent(source, rollToHitEvent).Paramters[EventParameters.RollToHit];
        rollToHitEvent.Release();

        RecLog.Log($"Roll to hit was {rollToHit}");

        GameEvent amAttacking = GameEventPool.Get(GameEventId.AmAttacking)
                                .With(EventParameters.RollToHit, rollToHit)
                                .With(EventParameters.Attack, weapon)
                                .With(EventParameters.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(weapon, amAttacking);
        checkWeaponAttack.Paramters.Add(EventParameters.DamageSource, source.ID);

        GameEvent getStatModForDamage = GameEventPool.Get(GameEventId.GetStat)
                                            .With(EventParameters.Value, 0);

        var damageList = checkWeaponAttack.GetValue<List<Damage>>(EventParameters.DamageList);
        int statBasedDamage = 0;
        bool addStatBasedDamage = false;
        switch (weaponType)
        {
            case TypeWeapon.Melee:
            case TypeWeapon.StrSpell:
                getStatModForDamage = getStatModForDamage.With(EventParameters.StatType, Stat.Str);
                addStatBasedDamage = true;
                break;
            case TypeWeapon.Finesse:
            case TypeWeapon.Ranged:
            case TypeWeapon.AgiSpell:
                getStatModForDamage = getStatModForDamage.With(EventParameters.StatType, Stat.Agi);
                addStatBasedDamage = true;
                break;
        }

        if(addStatBasedDamage)
        {
            statBasedDamage = source.FireEvent(getStatModForDamage).GetValue<int>(EventParameters.Value);
            damageList.First(d => d.DamageType == DamageType.Blunt || d.DamageType == DamageType.Slashing || d.DamageType == DamageType.Piercing).DamageAmount += statBasedDamage;
        }

        GameEvent attack = GameEventPool.Get(GameEventId.TakeDamage).With(checkWeaponAttack.Paramters);
        source.FireEvent(target, attack);
        attack.Release();

        if(weaponType != TypeWeapon.Melee && weaponType != TypeWeapon.Finesse)
        {
            GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                                                    .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
                                                    .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
            weapon.FireEvent(fireRangedWeapon).Release();
        }

        getStatModForDamage.Release();
        amAttacking.Release();

        if(useEnergy)
            source.FireEvent(source, GameEventPool.Get(GameEventId.UseEnergy)
                .With(EventParameters.Value, 1f)).Release(); //todo: temp energy value.  Energy value should come from the weapon probably
    }

    public static bool CastSpell(IEntity source, IEntity target, IEntity spellSource)
    {
        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                                .With(EventParameters.Value, 0);
        int mana = source.FireEvent(getMana).GetValue<int>(EventParameters.Value);

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, new HashSet<string>());

        HashSet<string> spells = spellSource.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameters.SpellList);
        foreach(string id in spells)
        {
            IEntity spell = EntityQuery.GetEntity(id);
            GameEvent manaCost = GameEventPool.Get(GameEventId.ManaCost)
                                    .With(EventParameters.Value, 0);
            int cost = spell.FireEvent(manaCost).GetValue<int>(EventParameters.Value);
            if (cost <= mana)
            {
                GameEvent getVisibleTiles = GameEventPool.Get(GameEventId.SelectTile)
                                                .With(EventParameters.TilePosition, WorldUtility.GetEntityPosition(target))
                                                .With(EventParameters.Value, false);
                spell.FireEvent(getVisibleTiles);

                Attack(source, target, spell, false);
                GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                                            .With(EventParameters.Effect, new Action<IEntity>((t) => 
                                                Attack(source, t, spell, false, false)));
                spell.FireEvent(affectArea);
                GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, cost);
                source.FireEvent(depleteMana);
                GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                                                    .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
                                                    .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                spell.FireEvent(fireRangedWeapon).Release();
                
                depleteMana.Release();
                affectArea.Release();
                getVisibleTiles.Release();
                manaCost.Release();
                getSpells.Release();
                getMana.Release();

                //GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy, new .With(EventParameters.Value, 1f));
                //source.FireEvent(useEnergy);
                return true;
            }
        }
        return false;
    }

    public static TypeWeapon GetWeaponType(IEntity weapon)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetWeaponType)
            .With(EventParameters.WeaponType, TypeWeapon.None);

        weapon.FireEvent(weapon, typedEvent);
        var retValue = typedEvent.GetValue<TypeWeapon>(EventParameters.WeaponType);
        typedEvent.Release();
        return retValue;
    }

    public static List<TypeWeapon> GetWeaponTypes(IEntity weapon)
    {
        var typedEvent = GameEventPool.Get(GameEventId.GetWeaponTypes)
            .With(EventParameters.WeaponTypeList, new List<TypeWeapon>());

        weapon.FireEvent(weapon, typedEvent);
        var retValue = typedEvent.GetValue<List<TypeWeapon>>(EventParameters.WeaponTypeList);
        typedEvent.Release();
        return retValue;
    }

    public static bool ICanTakeThem(int sourceValue, int targetValue)
    {
        if(targetValue <= (sourceValue + CombatRatingBuffer))
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
