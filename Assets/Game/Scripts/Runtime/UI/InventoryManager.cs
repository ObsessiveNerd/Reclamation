using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Eqiupment;

    public List<GameObject> InventorySlots;

    public void Open(GameObject source)
    {
        foreach(var item in source.GetComponent<Inventory>().GetInventory())
            InventorySlots[item.Key].GetComponent<InventorySlot>().SetItem(source, item.Value);

        Inventory.SetActive(true);
        Eqiupment.SetActive(true);
    }

    public void Close()
    {
        foreach(var slot in InventorySlots)
            slot.GetComponent<InventorySlot>().Clear();

        Inventory.SetActive(false);
        Eqiupment.SetActive(false);
    }
}
