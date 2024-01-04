using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestMono : EscapeableMono
{
    public Transform ChestContent;
    public Transform Inventories;

    List<GameObject> m_Inventories = new List<GameObject>();

    private GameObject m_Chest;
    private List<GameObject> m_Items = new List<GameObject>();

    public void Setup(GameObject chest)
    {
        if (chest == null)
            return;

        m_Chest = chest;
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);

        OpenedThisFrame();
    }

    public override void UpdateUI()
    {
        Cleanup();

        GameEvent getItems = GameEventPool.Get(GameEventId.GetItems)
                                .With(EventParameter.Items, new List<string>());

        List<string> itemIds = m_Chest.FireEvent(getItems).GetValue<List<string>>(EventParameter.Items);
        getItems.Release();

        //foreach (var itemId in itemIds)
        //{
        //    m_Items.Add(
        //        UIUtility.CreateItemGameObject(m_Chest, Services.EntityMapService.GetEntity(itemId), ChestContent));
        //}
    }

    public void Cleanup()
    {
        foreach (GameObject go in m_Items)
            Destroy(go);
        m_Items.Clear();
    }

    public override void OnEscape()
    {
        Cleanup();
        UIUtility.ClosePlayerInventory();
        gameObject.SetActive(false);
        m_Chest = null;
    }

    public void LootAll()
    {
        List<GameObject> items = new List<GameObject>(m_Items);
        //foreach(var item in items)
        //{
        //    GameObject itemEntity = item.GetComponent<InventoryItemMono>().ItemObject;
        //    GameEvent removeFromChest = GameEventPool.Get(GameEventId.RemoveFromInventory)
        //                                .With(EventParameter.Item, itemEntity.ID);
        //    m_Chest.FireEvent(removeFromChest);
        //    removeFromChest.Release();

        //    GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
        //                                            .With(EventParameter.Entity, itemEntity.ID);
        //    GameObject activePlayer = Services.EntityMapService.GetEntity(Services.WorldDataQuery.GetActivePlayer());
        //    activePlayer.FireEvent(addToInventory);
        //    addToInventory.Release();

        //    Services.WorldUIService.UpdateUI();
        //}
    }
}
