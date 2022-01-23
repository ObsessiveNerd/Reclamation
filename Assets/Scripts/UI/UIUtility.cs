using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static GameObject CreateItemGameObject(IEntity source, IEntity item, Transform parent)
    {
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
}
