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
        FireProjectile(gameEvent);
    }

    void FireProjectile(GameEvent gameEvent)
    {
        var source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        var target = gameEvent.GetValue<GameObject>(EventParameter.Target);

        var inventory = source.GetComponent<Inventory>();
        foreach (var item in inventory.InventoryItems)
        {
            var projectile = item.GetComponent<Projectile>();
            if(projectile != null && projectile.Type == RequiredAmmoType)
            {

                var projectileInstance = Services.EntityFactory.Create(item);

                var animatedProjectile = Instantiate(Resources.Load<GameObject>("AnimatedProjectile"));
                animatedProjectile.GetComponent<SpriteRenderer>().sprite = projectile.GetComponent<SpriteRenderer>().sprite;
                animatedProjectile.GetComponent<AnimatedProjectile>().Destination = target.transform.position;
                animatedProjectile.GetComponent<AnimatedProjectile>().EntityInstance = projectileInstance;
            }
        }
    }
}
