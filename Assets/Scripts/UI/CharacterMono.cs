using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterMono : MonoBehaviour
{
    //public GameObject CharacterManagerObject;

    public GameObject InventoryView;
    public GameObject EquipmentView;
    public GameObject CharacterStats;

    public void Setup(IEntity source)
    {
        InventoryView.GetComponent<InventoryManagerMono>().Setup(source);
        EquipmentView.GetComponent<EquipmentViewMono>().Setup(source);
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Setup(source);

    }

    public void Cleanup()
    {
        InventoryView.GetComponent<InventoryManagerMono>().Close();
        EquipmentView.GetComponent<EquipmentViewMono>().Close();
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Close();
    }
}
