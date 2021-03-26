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

        EventBuilder builder = new EventBuilder(GameEventId.AmAttacking)
                                .With(EventParameters.RollToHit, rollToHit)
                                .With(EventParameters.Attack, weapon)
                                .With(EventParameters.DamageList, new List<Damage>());

        GameEvent checkWeaponAttack = source.FireEvent(weapon, builder.CreateEvent());

        GameEvent attack = new GameEvent(GameEventId.TakeDamage, checkWeaponAttack.Paramters);
        source.FireEvent(target, attack);

        source.FireEvent(source, new GameEvent(GameEventId.UseEnergy, new KeyValuePair<string, object>(EventParameters.Value, 1f))); //todo: temp energy value.  Energy value should come from the weapon probably
    }

    public static TypeWeapon GetWeaponType(IEntity weapon)
    {
        TypeWeapon weaponType = (TypeWeapon)weapon.FireEvent(weapon, new GameEvent(GameEventId.GetWeaponType,
            new KeyValuePair<string, object>(EventParameters.WeaponType, TypeWeapon.None))).Paramters[EventParameters.WeaponType];
        return weaponType;
    }

    public static bool ValueIsWithinCRBuffer(int sourceValue, int targetValue)
    {
        if (targetValue >= sourceValue - CombatRatingBuffer && targetValue <= sourceValue + CombatRatingBuffer)
            return true;
        return false;
    }
}
