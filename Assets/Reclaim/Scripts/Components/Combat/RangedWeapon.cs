using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RangedWeapon : EntityComponent
{
    public ProjectileType RequiredAmmoType;

    void Start()
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
        RegisteredEvents.Add(GameEventId.FireProjectile, FireProjectile);
    }

    void PerformAttack(GameEvent gameEvent)
    {
        //var damages = GetComponents<Damage>();
        //var damageTaken = GameEventPool.Get(GameEventId.DamageTaken)
        //    .With(gameEvent.Paramters)
        //    .With(EventParameter.DamageList, damages);

        //gameEvent.GetValue<GameObject>(EventParameter.Target).FireEvent(damageTaken);

        //damageTaken.Release();

        FireProjectile(gameEvent);
    }

    void FireProjectile(GameEvent gameEvent)
    {
        var source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        var target = gameEvent.GetValue<GameObject>(EventParameter.Target);

        var inventory = source.GetComponent<Inventory>();
        foreach (var item in inventory.InventoryItems)
        {
            var projectile = item.Object.GetComponent<Projectile>();
            if(projectile != null && projectile.Type == RequiredAmmoType)
            {
                Debug.LogError($"Firing {RequiredAmmoType}");

                var projectileInstance = Services.EntityFactory.Create(projectile.gameObject);
                projectileInstance.transform.position = source.transform.position;
                projectileInstance.Show();
                projectile.Fire(target, gameEvent.GetValue<List<Damage>>(EventParameter.DamageList));
            }
        }
    }
}
