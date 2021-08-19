using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterManagerMono : EscapeableMono
{
    public GameObject CharacterManagerObject;

    public GameObject InventoryView;
    public GameObject EquipmentView;
    public GameObject CharacterStats;

    public void Setup(IEntity source)
    {
        UIManager.Push(this);
        CharacterManagerObject.SetActive(true);

        InventoryView.GetComponent<InventoryManagerMono>().Setup(source);
        EquipmentView.GetComponent<EquipmentViewMono>().Setup(source);
        CharacterStats.GetComponent<CharacterStatsMono>().Setup(source);
    }

    public override void OnEscape()
    {
        Cleanup();
        CharacterManagerObject.SetActive(false);
    }

    void Cleanup()
    {
        InventoryView.GetComponent<InventoryManagerMono>().Close();
        EquipmentView.GetComponent<EquipmentViewMono>().Close();
        //CharacterStats.GetComponent<CharacterStatsMono>().Cleanup();
    }
}
