using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    SO_Item Item;

    private void Start()
    {
        if (Item != null)
            SetItem(Item);
    }

    public void SetItem(SO_Item item)
    {
        GetComponent<Image>().sprite = item.Icon;
        GetComponent<SimpleTooltip>().Title = item.name;
        GetComponent<SimpleTooltip>().Body = item.GetDescription();
    }
}
