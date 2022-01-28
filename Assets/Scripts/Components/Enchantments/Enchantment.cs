using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment : Component
{
    public IEntity EnchantmentEntity;

    public Enchantment(string id)
    {
        EnchantmentEntity = EntityFactory.GetEntity(id);
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetEnchantments);
        RegisteredEvents.Add(GameEventId.GetInfo);

        if (EnchantmentEntity == null)
        {
            EnchantmentEntity = EntityFactory.CreateEntity(EntityFactory.GetRandomEnchantmentBPName(0));
            //EnchantmentEntity = new Actor("Enchantment");
            //EnchantmentEntity.AddComponent(new DealDamage(DamageType.Arcane, new Dice("1d8")));
            //EntityFactory.CreateTemporaryBlueprint(EnchantmentEntity.ID, EnchantmentEntity.Serialize());
        }
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions)
                .Add(new ContextMenuButton("Enchant", () =>
            {
                UIManager.ForcePop();
                UIManager.ForcePop();
                Services.WorldUIService.OpenEnchantmentUI(source, Self);
            }));
        }

        else if(gameEvent.ID == GameEventId.GetEnchantments)
        {
            gameEvent.GetValue<List<string>>(EventParameters.Enchantments).Add(EnchantmentEntity.ID);
        }
        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            EnchantmentEntity.FireEvent(gameEvent);
        }
    }
}

public class DTO_Enchantment : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Enchantment(data.Split('=')[1]);
    }

    public string CreateSerializableData(IComponent component)
    {
        Enchantment e = (Enchantment)component;
        return $"{nameof(Enchantment)}: {nameof(e.EnchantmentEntity.ID)}={e.EnchantmentEntity.ID}";
    }
}
