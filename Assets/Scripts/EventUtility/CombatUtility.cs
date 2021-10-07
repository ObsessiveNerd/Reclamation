using System.Collections.Generic;

public static class CombatUtility
{
    static int CombatRatingBuffer => 4;

    public static void Attack(IEntity source, IEntity target, IEntity weapon)
    {
        TypeWeapon weaponType = GetWeaponType(weapon);
        RecLog.Log($"{source.Name} is performing a {weaponType} attack with {weapon.Name}");
        if (weaponType == TypeWeapon.None)
            return;
        int rollToHit = (int)source.FireEvent(source, new GameEvent(GameEventId.RollToHit, new KeyValuePair<string, object>(EventParameters.RollToHit, 0),
                                                                          new KeyValuePair<string, object>(EventParameters.WeaponType, weaponType))).Paramters[EventParameters.RollToHit];
        RecLog.Log($"Roll to hit was {rollToHit}");

        EventBuilder builder = EventBuilderPool.Get(GameEventId.AmAttacking)
                                .With(EventParameters.RollToHit, rollToHit)
                                .With(EventParameters.Attack, weapon)
                                .With(EventParameters.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(weapon, builder.CreateEvent());
        checkWeaponAttack.Paramters.Add(EventParameters.DamageSource, source.ID);

        GameEvent attack = new GameEvent(GameEventId.TakeDamage, checkWeaponAttack.Paramters);
        source.FireEvent(target, attack);

        if(weaponType != TypeWeapon.Melee && weaponType != TypeWeapon.Finesse)
        {
            EventBuilder fireRangedWeapon = EventBuilderPool.Get(GameEventId.FireRangedAttack)
                                                    .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
                                                    .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
            weapon.FireEvent(fireRangedWeapon.CreateEvent());
        }
        source.FireEvent(source, new GameEvent(GameEventId.UseEnergy, new KeyValuePair<string, object>(EventParameters.Value, 1f))); //todo: temp energy value.  Energy value should come from the weapon probably
    }

    public static bool CastSpell(IEntity source, IEntity target, IEntity spellSource)
    {
        EventBuilder getMana = EventBuilderPool.Get(GameEventId.GetMana)
                                .With(EventParameters.Value, 0);
        int mana = source.FireEvent(getMana.CreateEvent()).GetValue<int>(EventParameters.Value);

        EventBuilder getSpells = EventBuilderPool.Get(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, new HashSet<string>());

        HashSet<string> spells = spellSource.FireEvent(getSpells.CreateEvent()).GetValue<HashSet<string>>(EventParameters.SpellList);
        foreach(string id in spells)
        {
            IEntity spell = EntityQuery.GetEntity(id);
            EventBuilder manaCost = EventBuilderPool.Get(GameEventId.ManaCost)
                                    .With(EventParameters.Value, 0);
            int cost = spell.FireEvent(manaCost.CreateEvent()).GetValue<int>(EventParameters.Value);
            if (cost <= mana)
            {
                Attack(source, target, spell);
                GameEvent depleteMana = new GameEvent(GameEventId.DepleteMana, new KeyValuePair<string, object>(EventParameters.Mana, cost));
                source.FireEvent(depleteMana);
                EventBuilder fireRangedWeapon = EventBuilderPool.Get(GameEventId.FireRangedAttack)
                                                    .With(EventParameters.Entity, WorldUtility.GetGameObject(source).transform.position)
                                                    .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                spell.FireEvent(fireRangedWeapon.CreateEvent());
                GameEvent useEnergy = new GameEvent(GameEventId.UseEnergy, new KeyValuePair<string, object>(EventParameters.Value, 1f));
                source.FireEvent(useEnergy);
                return true;
            }
        }
        return false;
    }

    public static TypeWeapon GetWeaponType(IEntity weapon)
    {
        TypeWeapon weaponType = (TypeWeapon)weapon.FireEvent(weapon, new GameEvent(GameEventId.GetWeaponType,
            new KeyValuePair<string, object>(EventParameters.WeaponType, TypeWeapon.None))).Paramters[EventParameters.WeaponType];
        return weaponType;
    }

    public static bool ICanTakeThem(int sourceValue, int targetValue)
    {
        if(targetValue < (sourceValue + CombatRatingBuffer))
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
