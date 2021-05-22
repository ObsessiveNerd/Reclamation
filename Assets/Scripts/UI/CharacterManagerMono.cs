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
        CharacterManagerObject.SetActive(true);
        UpdateUI(source.ID);
    }

    public void UpdateUI(string id)
    {
        Cleanup();
        IEntity source = EntityQuery.GetEntity(id);
        InventoryView.GetComponent<InventoryManagerMono>().Setup(source);
        EquipmentView.GetComponent<EquipmentViewMono>().Setup(source);
        CharacterStats.GetComponent<CharacterStatsMono>().Setup(source);
    }

    protected override void OnEscape()
    {
        CharacterManagerObject.SetActive(false);
        Cleanup();
    }

    void Cleanup()
    {
        InventoryView.GetComponent<InventoryManagerMono>().Cleanup();
        EquipmentView.GetComponent<EquipmentViewMono>().Cleanup();
        CharacterStats.GetComponent<CharacterStatsMono>().Cleanup();
    }
}
