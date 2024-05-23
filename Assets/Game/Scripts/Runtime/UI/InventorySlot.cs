using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;

public class InventorySlot : TooltipTrigger, IPointerDownHandler
{
    [SerializeField]
    SO_Item Item;

    public Image Icon;
    private ContextMenu m_ContextMenu;
    private GameObject m_InventorySource;

    protected override bool ShouldShowTooltip
    {
        get
        {
            return Item != null;
        }
    }

    protected override void Start()
    {
        this.m_ContextMenu = WidgetUtility.Find<ContextMenu>("ContextMenu");

        if (Item != null)
            SetItem(FindFirstObjectByType<PlayerInputController>().gameObject, Item);

        base.Start();
    }

    public void SetItem(GameObject source, SO_Item item)
    {
        m_InventorySource = source;
        Item = item;
        Icon.sprite = item.Icon;
        Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 255f);
        tooltipTitle = item.name;
        tooltip = item.GetDescription();
    }

    public void Clear()
    {
        m_InventorySource = null;
        Item = null;
        Icon.sprite = null;
        Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0f);
        tooltipTitle = "";
        tooltip = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.m_ContextMenu.Clear();
            m_ContextMenu.AddMenuItem("Equip", delegate { Debug.Log("Equip " + Item.name); });

            m_ContextMenu.AddMenuItem("Drop", delegate
            {
                m_InventorySource.GetComponent<Inventory>().RemoveFromInventory(Item);
                Spawner.Instance.Spawn(Item, m_InventorySource.transform.position);
                Clear();
            });
            this.m_ContextMenu.Show();
        }
    }
}
