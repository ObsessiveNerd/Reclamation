using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterMono : MonoBehaviour
{
    public GameObject EquipmentView;
    public GameObject CharacterStats;
    public GameObject InfoView;
    public GameObject AbilitiesView;

    public void ToggleSelected(string toggleName)
    {
        switch(toggleName)
        {
            case "Equipment":
                CharacterStats.SetActive(false);
                EquipmentView.SetActive(true);
                AbilitiesView.SetActive(false);
                InfoView.SetActive(false);
                break;
            case "Stats":
                CharacterStats.SetActive(true);
                EquipmentView.SetActive(false);
                InfoView.SetActive(false);
                AbilitiesView.SetActive(false);
                break;
            case "Info":
                CharacterStats.SetActive(false);
                EquipmentView.SetActive(false);
                InfoView.SetActive(true);
                AbilitiesView.SetActive(false);
                break;
            case "Abilities":
                CharacterStats.SetActive(false);
                EquipmentView.SetActive(false);
                InfoView.SetActive(false);
                AbilitiesView.SetActive(true);
                break;
        }
        Services.WorldUIService.UpdateUI();
    }
}
