using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : TooltipTrigger
{
    [SerializeField]
    SO_Item Item;

    protected override void Start()
    {
        if (Item != null)
            SetItem(Item);

        tooltipTitle = Item.name;
        tooltip = Item.GetDescription();

        base.Start();
    }

    public void SetItem(SO_Item item)
    {
        GetComponent<Image>().sprite = item.Icon;
    }
}
