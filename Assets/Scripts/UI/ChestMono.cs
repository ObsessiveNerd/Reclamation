using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestMono : EscapeableMono, IUpdatableUI
{
    public GameObject ChestUI;
    public Transform ChestContent;

    private InventoryManagerMono m_Inventory;
    private IEntity m_Chest;
    private IEntity m_Character;

    private List<GameObject> m_Items = new List<GameObject>();

    public void Init(IEntity chest, IEntity character)
    {
        if (chest == null || character == null)
            return;

        m_Chest = chest;
        m_Character = character;

        ChestUI.SetActive(true);
        WorldUtility.RegisterUI(this);
        UIManager.Push(this);
        UpdateUI(m_Character);
    }

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        GameObject go = Resources.Load<GameObject>("UI/InventoryItem");
        EventBuilder getItems = EventBuilderPool.Get(GameEventId.GetItems)
                                .With(EventParameters.Items, new List<string>());

        List<string> itemIds = m_Chest.FireEvent(getItems.CreateEvent()).GetValue<List<string>>(EventParameters.Items);
        foreach(var itemId in itemIds)
        {
            var item = EntityQuery.GetEntity(itemId);

            Sprite sprite = item.FireEvent(item, new GameEvent(GameEventId.GetPortrait, 
                new KeyValuePair<string, object>(EventParameters.RenderSprite, null))).GetValue<Sprite>(EventParameters.RenderSprite);
            if(sprite != null)
            {
                GameObject spriteGo = Instantiate(go);
                Image spriteRenderer = spriteGo.GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(ChestContent);
                spriteGo.AddComponent<ChestItemMono>().Init(m_Chest, item, newSource);
                m_Items.Add(spriteGo);
            }
        }

        m_Inventory = ChestUI.GetComponentInChildren<InventoryManagerMono>();
        m_Inventory.Setup(newSource);
    }

    public void Cleanup()
    {
        foreach (GameObject go in m_Items)
                Destroy(go);
        m_Items.Clear();
    }

    public override void OnEscape()
    {
        m_Inventory?.Cleanup();
        Cleanup();
        WorldUtility.UnRegisterUI(this);
        ChestUI.SetActive(false);
        m_Chest = null;
        m_Character = null;
    }
}
