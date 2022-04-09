using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static GameObject CreateItemGameObject(IEntity source, IEntity item, Transform parent)
    {
        if (source == null || item == null || parent == null)
        {
            throw new System.Exception("Cannot create inventory item with null parameters");
        }

        GameObject spriteGoResource = Resources.Load<GameObject>("Prefabs/UI/InventoryItem");

        if (item == null)
            return null;

        GameEvent getSprite = GameEventPool.Get(GameEventId.GetPortrait)
                .With(EventParameters.RenderSprite, null);

        Sprite sprite = item.FireEvent(item, getSprite).GetValue<Sprite>(EventParameters.RenderSprite);
        getSprite.Release();

        if (sprite != null)
        {
            GameObject spriteGo = GameObject.Instantiate(spriteGoResource);
            Image spriteRenderer = spriteGo.GetComponent<Image>();
            spriteRenderer.sprite = sprite;
            spriteGo.transform.SetParent(parent);
            spriteGo.GetComponent<InventoryItemMono>().Init(source, item);
            return spriteGo;
        }

        return null;
    }

    static List<GameObject> m_Inventories = new List<GameObject>();

    public static void ClosePlayerInventory()
    {
        foreach (var inventory in m_Inventories)
        {
            inventory.GetComponent<InventoryManagerMono>().Close();
            GameObject.Destroy(inventory);
        }

        m_Inventories.Clear();
    }

    public static List<GameObject> CreatePlayerInventories(Transform parent)
    {
        if (m_Inventories.Count > 0)
            ClosePlayerInventory();

        m_Inventories = new List<GameObject>();
        GameObject inventory = Resources.Load<GameObject>("Prefabs/UI/Inventory");
        foreach (string id in Services.WorldDataQuery.GetPlayableCharacters())
        {
            GameObject go = GameObject.Instantiate(inventory);
            go.GetComponent<InventoryManagerMono>().Setup(Services.EntityMapService.GetEntity(id));
            go.transform.SetParent(parent, false);
            m_Inventories.Add(go);
        }
        return m_Inventories;
    }
}
