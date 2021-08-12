using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManagerMono : MonoBehaviour, IUpdatableUI
{
    public Transform InventoryView;
    IEntity m_Source;
    List<GameObject> m_Items = new List<GameObject>();

    public void Setup(IEntity source)
    {
        m_Source = source;

        WorldUtility.RegisterUI(this);
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
                m_Items.Add(spriteGo);
            }
        }
    }

    public void Cleanup()
    {
        foreach (GameObject go in m_Items)
                Destroy(go);
        m_Items.Clear();
    }

    public void UpdateUI()
    {
        Cleanup();
        Setup(m_Source);
    }

    public void Close()
    {
        Cleanup();
        WorldUtility.UnRegisterUI(this);
    }
}
