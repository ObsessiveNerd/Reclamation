using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RangedWeaponData : ComponentData
{
    public ProjectileType RequiredAmmoType;
}

public class RangedWeapon : EntityComponent
{
    public RangedWeaponData Data = new RangedWeaponData();

    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
        RegisteredEvents.Add(GameEventId.FireProjectile, FireProjectile);
        if (data != null)
            Data = data as RangedWeaponData;
    }
    public override IComponentData GetData()
    {
        return Data;
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
            if(projectile != null && projectile.Type == Data.RequiredAmmoType)
            {

                var projectileInstance = Services.EntityFactory.Create(item, source.transform.position);
                projectileInstance.Show();
                var setDestination = GameEventPool.Get(GameEventId.MoveEntity)
                    .With(EventParameter.CanMove, true)
                    .With(EventParameter.TilePosition, new Point(target.transform.position));
                projectileInstance.FireEvent(setDestination);
                setDestination.Release();

                //var animatedProjectile = Instantiate(Resources.Load<GameObject>("AnimatedProjectile"));
                //animatedProjectile.GetComponent<SpriteRenderer>().sprite = projectile.GetComponent<SpriteRenderer>().sprite;
                //animatedProjectile.GetComponent<AnimatedProjectile>().Destination = target.transform.position;
                //animatedProjectile.GetComponent<AnimatedProjectile>().EntityInstance = projectileInstance;
            }
        }
    }
}
