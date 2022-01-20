using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManagerMono : MonoBehaviour//, IUpdatableUI
{
    public Transform InventoryView;
    IEntity m_Source;
    List<GameObject> m_Items = new List<GameObject>();

    public void Setup(IEntity source)
    {
        if(source != null)
            m_Source = source;

        Cleanup();

        //WorldUtility.RegisterUI(this);
        GameEvent getCurrentInventory = GameEventPool.Get(GameEventId.GetCurrentInventory)
                                            .With(EventParameters.Value, new List<IEntity>());

        List<IEntity> inventory = m_Source.FireEvent(getCurrentInventory).GetValue<List<IEntity>>(EventParameters.Value);
        getCurrentInventory.Release();

        GameObject spriteGoResource = Resources.Load<GameObject>("UI/InventoryItem");
        foreach(var item in inventory)
        {
            if (item == null)
                continue;

            GameEvent getSprite = GameEventPool.Get(GameEventId.GetPortrait)
                .With(EventParameters.RenderSprite, null);
            
            Sprite sprite = item.FireEvent(item, getSprite).GetValue<Sprite>(EventParameters.RenderSprite);
            getSprite.Release();

            if(sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(InventoryView);
                spriteGo.AddComponent<InventoryItemMono>().Init(m_Source, item);
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

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        Setup(newSource);
    }

    public void Close()
    {
        Cleanup();
        //WorldUtility.UnRegisterUI(this);
    }
}
