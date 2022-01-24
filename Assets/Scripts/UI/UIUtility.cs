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

        GameObject spriteGoResource = Resources.Load<GameObject>("UI/InventoryItem");

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
            spriteGo.AddComponent<InventoryItemMono>().Init(source, item);
            return spriteGo;
        }

        return null;
    }

    public static List<GameObject> CreatePlayerInventories(Transform parent)
    {
        List<GameObject> inventories = new List<GameObject>();
        GameObject inventory = Resources.Load<GameObject>("UI/Inventory");
        foreach (string id in Services.WorldDataQuery.GetPlayableCharacters())
        {
            GameObject go = GameObject.Instantiate(inventory);
            go.GetComponent<InventoryManagerMono>().Setup(Services.EntityMapService.GetEntity(id));
            go.transform.SetParent(parent);
            inventories.Add(go);
        }
        return inventories;
    }
}
