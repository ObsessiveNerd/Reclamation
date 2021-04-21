using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManagerMono : MonoBehaviour
{
    public Transform InventoryView;
    IEntity m_Source;
    public void Setup(IEntity source)
    {
        m_Source = source;
        EventBuilder getCurrentInventory = new EventBuilder(GameEventId.GetCurrentInventory)
                                            .With(EventParameters.Value, new List<IEntity>());

        List<IEntity> inventory = m_Source.FireEvent(getCurrentInventory.CreateEvent()).GetValue<List<IEntity>>(EventParameters.Value);

        GameObject spriteGoResource = Resources.Load<GameObject>("UI/InventoryItem");
        foreach(var item in inventory)
        {
            Sprite sprite = item.FireEvent(item, new GameEvent(GameEventId.GetInfo, 
                new KeyValuePair<string, object>(EventParameters.RenderSprite, null))).GetValue<Sprite>(EventParameters.RenderSprite);
            if(sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(InventoryView);
                spriteGo.AddComponent<InventoryItemMono>().Init(source, item);
            }
        }
    }

    public void Cleanup()
    {
        foreach (Transform go in InventoryView.GetComponentsInChildren<Transform>())
            if (InventoryView.transform != go)
                Destroy(go.gameObject);
    }
}
