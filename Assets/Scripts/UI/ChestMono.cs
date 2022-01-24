using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestMono : EscapeableMono//, IUpdatableUI
{
    public GameObject ChestUI;
    public Transform ChestContent;

    public Transform Inventories;

    List<GameObject> m_Inventories = new List<GameObject>();

    //private InventoryManagerMono m_Inventory;
    private IEntity m_Chest;

    private List<GameObject> m_Items = new List<GameObject>();

    public void Init(IEntity chest)
    {
        if (chest == null)
            return;

        m_Chest = chest;
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);

        ChestUI.SetActive(true);
        //WorldUtility.RegisterUI(this);
        UIManager.Push(this);
        UpdateUI();
    }

    public void UpdateUI()
    {
        Cleanup();


        //GameObject go = Resources.Load<GameObject>("UI/InventoryItem");
        GameEvent getItems = GameEventPool.Get(GameEventId.GetItems)
                                .With(EventParameters.Items, new List<string>());

        List<string> itemIds = m_Chest.FireEvent(getItems).GetValue<List<string>>(EventParameters.Items);
        getItems.Release();

        foreach (var itemId in itemIds)
        {
            m_Items.Add(
                UIUtility.CreateItemGameObject(m_Chest, Services.EntityMapService.GetEntity(itemId), ChestContent));
        }

        //m_Inventory = ChestUI.GetComponentInChildren<InventoryManagerMono>();
        //m_Inventory.Setup(newSource);
    }

    public void Cleanup()
    {
        foreach (GameObject go in m_Items)
            Destroy(go);
        m_Items.Clear();
    }

    public override void OnEscape()
    {
        //m_Inventory?.Cleanup();
        Cleanup();
        foreach (GameObject go in m_Inventories)
        {
            go.GetComponent<InventoryManagerMono>().Close();
            Destroy(go);
        }
        m_Inventories.Clear();
        //WorldUtility.UnRegisterUI(this);
        ChestUI.SetActive(false);
        m_Chest = null;
    }
}
