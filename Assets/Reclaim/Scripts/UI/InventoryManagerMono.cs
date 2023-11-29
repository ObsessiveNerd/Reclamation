using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class InventoryManagerMono : UpdatableUI, IDropHandler
{
    public Action ItemDropped;
    public TextMeshProUGUI Name;
    public Transform InventoryView;
    public Image CharacterImage;
    
    public IEntity Source;
    Dictionary<IEntity, GameObject> m_Items = new Dictionary<IEntity,GameObject>();
    string m_ActiveFilter = "All";

    public void ClearCallback()
    {
        ItemDropped = null;
    }

    public void Setup(IEntity source)
    {
        if (source != null)
            Source = source;
    }

    public void Cleanup()
    {
        List<IEntity> keysToRemove = new List<IEntity>();
        foreach (IEntity entity in m_Items.Keys)
        {
            Destroy(m_Items[entity]);
            keysToRemove.Add(entity);
        }

        foreach (var key in keysToRemove)
            m_Items.Remove(key);
    }

    public override void UpdateUI()
    {
        Cleanup();
        Name.text = Source.Name;

        GameEvent getSprite = GameEventPool.Get(GameEventId.GetSprite)
                                            .With(EventParameter.RenderSprite, null);
        CharacterImage.sprite = Source.FireEvent(getSprite).GetValue<Sprite>(EventParameter.RenderSprite);
        getSprite.Release();

        List<IEntity> inventory = new List<IEntity>();
        Dictionary<IEntity, IEntity> itemToPlayerSource = new Dictionary<IEntity, IEntity>();
        foreach (var player in Services.PlayerManagerService.GetAllPlayers())
        {
            GameEvent getCurrentInventory = GameEventPool.Get(GameEventId.GetCurrentInventory)
                                            .With(EventParameter.Value, new List<IEntity>());

            var items = player.FireEvent(getCurrentInventory).GetValue<List<IEntity>>(EventParameter.Value);
            foreach (var item in items)
                itemToPlayerSource.Add(item, player);
            inventory.AddRange(items);
            getCurrentInventory.Release();
        }

        inventory = Filter(inventory);
        inventory.Sort(new EntityComparer());

        foreach (var item in inventory)
        {
            if (!m_Items.ContainsKey(item))
            {
                if(item.HasComponent(typeof(Stackable)))
                {
                    if(!m_Items.Keys.Any(k => k.Name == item.Name))
                       m_Items.Add(item, UIUtility.CreateItemGameObject(itemToPlayerSource[item], item, InventoryView));
                }
                else
                    m_Items.Add(item, UIUtility.CreateItemGameObject(itemToPlayerSource[item], item, InventoryView));
            }
        }
    }

    public void Close()
    {
        Cleanup();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || eventData.pointerDrag.GetComponent<InventoryItemMono>() == null)
            return;

        eventData.pointerDrag.GetComponent<InventoryItemMono>().AllowConxtMenuOptions = true;
        IEntity source = eventData.pointerDrag.GetComponent<InventoryItemMono>().Source;
        IEntity item = eventData.pointerDrag.GetComponent<InventoryItemMono>().ItemObject;

        GameEvent getBodyPartForEquipment = GameEventPool.Get(GameEventId.GetBodyPartType)
            .With(EventParameter.BodyPart, BodyPart.None);
        item.FireEvent(getBodyPartForEquipment);
        BodyPart equipmentBodyPart = getBodyPartForEquipment.GetValue<BodyPart>(EventParameter.BodyPart);
        getBodyPartForEquipment.Release();

        if (equipmentBodyPart != BodyPart.None)
        {
            GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameter.Entity, source.ID)
                .With(EventParameter.EntityType, equipmentBodyPart)
                .With(EventParameter.Item, item.ID);

            source.FireEvent(unEquip);
            unEquip.Release();
        }

        if (source != Source)
        {
            GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameter.Item, item.ID);
            source.FireEvent(removeFromInventory);
            removeFromInventory.Release();
            //Services.WorldUIService.UpdateUI(source.ID);
        }

        GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                    .With(EventParameter.Entity, item.ID);
        Source.FireEvent(addToInventory);
        addToInventory.Release();

        if (!m_Items.ContainsKey(item))
        {
            Debug.Log($"Item {item.Name} is getting added to inventory");
            m_Items.Add(item, eventData.pointerDrag);
        }
        else
        {
            Debug.Log($"Destroy {item.Name}");
            Destroy(eventData.pointerDrag);
        }

        ItemDropped?.Invoke();
        Services.WorldUIService.UpdateUI();
    }

    public void FilterShownEquipment(string filter)
    {
        m_ActiveFilter = filter;
        UpdateUI();
    }

    List<IEntity> Filter(List<IEntity> allEquipment)
    {
        switch (m_ActiveFilter)
        {
            case "All":
                //Do nothing
                break;
            case "Armor":
                allEquipment = allEquipment.Where(e => e.HasComponent(typeof(Armor))).ToList();
                break;
            case "Weapons":
                allEquipment = allEquipment.Where(e => e.HasComponent(typeof(WeaponType))).ToList();
                break;
            case "SpellEquipment":
                allEquipment = allEquipment.Where(e => e.HasComponent(typeof(SpellContainer))).ToList();
                break;
            case "Trinkets":
                allEquipment = allEquipment.Where(e =>
                {
                    if (e.HasComponent(typeof(Equipment)) && (e.GetComponent<Equipment>().PreferredBodyPartWhenEquipped == BodyPart.Finger || e.GetComponent<Equipment>().PreferredBodyPartWhenEquipped == BodyPart.Neck))
                        return true;
                    return false;
                }).ToList();
                break;
            case "Cloaks":
                allEquipment = allEquipment.Where(e =>
                {
                    if (e.HasComponent(typeof(Equipment)) && e.GetComponent<Equipment>().PreferredBodyPartWhenEquipped == BodyPart.Back)
                        return true;
                    return false;
                }).ToList();
                break;
            case "Shields":
                allEquipment = allEquipment.Where(e =>
                {
                    if (e.HasComponent(typeof(Armor)) && e.HasComponent(typeof(Equipment)) && e.GetComponent<Equipment>().PreferredBodyPartWhenEquipped == BodyPart.Arm)
                        return true;
                    return false;
                }).ToList();
                break;
            case "Potions":
                allEquipment = allEquipment.Where(e => e.HasComponent(typeof(Potion))).ToList();
                break;
        }

        return allEquipment;
    }
}
