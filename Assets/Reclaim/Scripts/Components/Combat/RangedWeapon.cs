using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RangedWeaponData : EntityComponent
{
    [SerializeField]
    public ProjectileType RequiredAmmoType;

    [SerializeField]
    public Type MonobehaviorType = typeof(RangedWeapon);
    public override void WakeUp()
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
        var source = gameEvent.GetValue<Entity>(EventParameter.Source);
        var target = gameEvent.GetValue<Entity>(EventParameter.Target);

        var inventory = source.GetComponent<InventoryData>();
        foreach (var item in inventory.InventoryEntities)
        {
            var projectile = item.GetComponent<ProjectileData>();
            if(projectile != null && projectile.Type == RequiredAmmoType)
            {

                //var projectileInstance = Services.EntityFactory.Create(item, source.transform.position);
                //projectileInstance.Show();
                //var setDestination = GameEventPool.Get(GameEventId.MoveEntity)
                //    .With(EventParameter.CanMove, true)
                //    .With(EventParameter.TilePosition, new Point(target.transform.position));
                //projectileInstance.FireEvent(setDestination);
                //setDestination.Release();

                //var animatedProjectile = Instantiate(Resources.Load<GameObject>("AnimatedProjectile"));
                //animatedProjectile.GetComponent<SpriteRenderer>().sprite = projectile.GetComponent<SpriteRenderer>().sprite;
                //animatedProjectile.GetComponent<AnimatedProjectile>().Destination = target.transform.position;
                //animatedProjectile.GetComponent<AnimatedProjectile>().EntityInstance = projectileInstance;
            }
        }
    }

}

public class RangedWeapon : ComponentBehavior<RangedWeaponData>
{

}
