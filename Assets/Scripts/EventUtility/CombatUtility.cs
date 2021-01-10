using System.Collections.Generic;

public static class CombatUtility
{
    public static void Attack(IEntity source, IEntity target, IEntity weapon)
    {
        TypeWeapon weaponType = GetWeaponType(weapon);
        RecLog.Log($"{source.Name} is performing a {weaponType} attack");
        if (weaponType == TypeWeapon.None)
            return;
        int rollToHit = (int)source.FireEvent(source, new GameEvent(GameEventId.RollToHit, new KeyValuePair<string, object>(EventParameters.RollToHit, 0),
                                                                          new KeyValuePair<string, object>(EventParameters.WeaponType, weaponType))).Paramters[EventParameters.RollToHit];
        RecLog.Log($"Roll to hit was {rollToHit}");
        GameEvent checkWeaponAttack = source.FireEvent(weapon, new GameEvent(GameEventId.AmAttacking, new KeyValuePair<string, object>(EventParameters.RollToHit, rollToHit),
                                                      new KeyValuePair<string, object>(EventParameters.Attack, weapon),
                                                      new KeyValuePair<string, object>(EventParameters.DamageList, new List<Damage>()),
                                                      new KeyValuePair<string, object>(EventParameters.AdditionalGameEvents, new List<GameEvent>())));

        GameEvent attack = new GameEvent(GameEventId.TakeDamage, checkWeaponAttack.Paramters);
        source.FireEvent(target, attack);
        foreach (var ge in (List<GameEvent>)checkWeaponAttack.Paramters[EventParameters.AdditionalGameEvents])
            source.FireEvent(target, ge);

        source.FireEvent(source, new GameEvent(GameEventId.UseEnergy, new KeyValuePair<string, object>(EventParameters.Value, 1f))); //todo: temp energy value.  Energy value should come from the weapon probably
    }

    public static TypeWeapon GetWeaponType(IEntity weapon)
    {
        TypeWeapon weaponType = (TypeWeapon)weapon.FireEvent(weapon, new GameEvent(GameEventId.GetWeaponType,
            new KeyValuePair<string, object>(EventParameters.WeaponType, null))).Paramters[EventParameters.WeaponType];
        return weaponType;
    }
}
