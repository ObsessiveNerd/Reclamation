using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemContainer : EntityComponent
{
    public Dictionary<string, GameObject> IDToEntityMap = new Dictionary<string, GameObject>();

    public ItemContainer(string itemNames)
    {
        foreach(var item in EntityFactory.GetEntitiesFromArray(itemNames))
            IDToEntityMap[item.InternalName] = item;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.GetItems);
        RegisteredEvents.Add(GameEventId.AddItem);
        RegisteredEvents.Add(GameEventId.AddItems);
        RegisteredEvents.Add(GameEventId.DropItemsOnMap);
        RegisteredEvents.Add(GameEventId.AddItemsToInventory);
        RegisteredEvents.Add(GameEventId.Interact);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetItems)
        {
            gameEvent.Paramters[EventParameter.Items] = IDToEntityMap.Keys.ToList();
        }
        else if(gameEvent.ID == GameEventId.DropItemsOnMap)
        {
            Point p = WorldUtility.GetEntityPosition(Self);
            foreach (var e in IDToEntityMap.Values)
                Spawner.Spawn(e, p);
            IDToEntityMap.Clear();
            Spawner.Despawn(Self);
        }
        else if(gameEvent.ID == GameEventId.AddItemsToInventory)
        {
            //TODO
        }
        else if(gameEvent.ID == GameEventId.AddItem)
        {
            string name = gameEvent.GetValue<string>(EventParameter.Item);
            AddItem(name);
        }
        else if(gameEvent.ID == GameEventId.AddItems)
        {
            List<string> names = gameEvent.GetValue<List<string>>(EventParameter.Items);
            foreach(var name in names)
                AddItem(name);
        }
        else if(gameEvent.ID == GameEventId.RemoveFromInventory)
        {
            string id = gameEvent.GetValue<string>(EventParameter.Item);
            if(IDToEntityMap.ContainsKey(id))
                IDToEntityMap.Remove(id);
        }
        else if(gameEvent.ID == GameEventId.Interact)
        {
            string characterId = gameEvent.GetValue<string>(EventParameter.Entity);
            if (!WorldUtility.IsActivePlayer(characterId))
                return;

            //TODO
            StringBuilder sb = new StringBuilder("Container holds these items: \n");
            foreach (var item in IDToEntityMap.Values)
                sb.AppendLine(item.Name);
            Debug.Log(sb.ToString());

            GameEvent uiOpened = GameEventPool.Get(GameEventId.OpenUI);
            Services.EntityMapService.GetEntity(characterId).FireEvent(uiOpened);
            uiOpened.Release();

            Services.WorldUIService.OpenChestUI(Self, Services.EntityMapService.GetEntity(characterId));
        }
    }

    void AddItem(string name)
    {
        GameObject entity = EntityFactory.CreateEntity(name);
        if (entity == null)
            entity = EntityQuery.GetEntity(name);
        if (entity != null)
            IDToEntityMap.Add(entity.ID, entity);
    }
}

public class DTO_ItemContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (value.Contains("="))
            value = value.Split('=')[1];
        Component = new ItemContainer(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        ItemContainer ic = (ItemContainer)component;
        StringBuilder itemNameBuilder = new StringBuilder();
        foreach (var name in ic.IDToEntityMap.Keys)
        {
            itemNameBuilder.Append($"<{name}>,");
            EntityFactory.CreateTemporaryBlueprint(ic.IDToEntityMap[name].ID, ic.IDToEntityMap[name].Serialize());
        }
        return $"{nameof(ItemContainer)}: [{itemNameBuilder.ToString().TrimEnd(',')}]";
    }
}
