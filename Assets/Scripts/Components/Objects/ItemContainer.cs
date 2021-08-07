using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemContainer : Component
{
    public Dictionary<string, IEntity> ItemNameToIdMap = new Dictionary<string, IEntity>();

    public ItemContainer(string itemNames)
    {
        foreach(var item in EntityFactory.GetEntitiesFromArray(itemNames))
            ItemNameToIdMap[item.Name] = item;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetItems);
        RegisteredEvents.Add(GameEventId.AddItem);
        RegisteredEvents.Add(GameEventId.DropItemsOnMap);
        RegisteredEvents.Add(GameEventId.AddItemsToInventory);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetItems)
        {
            gameEvent.Paramters[EventParameters.Item] = ItemNameToIdMap.Values.ToList();
        }
        else if(gameEvent.ID == GameEventId.DropItemsOnMap)
        {
            Point p = WorldUtility.GetEntityPosition(Self);
            foreach (var e in ItemNameToIdMap.Values)
                Spawner.Spawn(e, p);
            ItemNameToIdMap.Clear();
            Spawner.Despawn(Self);
        }
        else if(gameEvent.ID == GameEventId.AddItemsToInventory)
        {
            //TODO
        }
        else if(gameEvent.ID == GameEventId.AddItem)
        {
            string name = gameEvent.GetValue<string>(EventParameters.Item);
            IEntity entity = EntityFactory.CreateEntity(name);
            if(entity != null)
                ItemNameToIdMap.Add(name, entity);
        }
    }
}

public class DTO_ItemContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new ItemContainer(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        ItemContainer ic = (ItemContainer)component;
        StringBuilder itemNameBuilder = new StringBuilder();
        foreach (var name in ic.ItemNameToIdMap.Keys)
            itemNameBuilder.Append($"<{name}>,");
        return $"{nameof(ItemContainer)}: [{itemNameBuilder.ToString().TrimEnd(',')}]";
    }
}
