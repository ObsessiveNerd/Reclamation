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
    public CharacterTab Tab;


    public void Setup(IEntity source)
    {
        EquipmentView.GetComponent<EquipmentViewMono>().Setup(source);
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Setup(source);
        InfoView.GetComponentInChildren<PlayerInfoMono>().Setup(source);
        AbilitiesView.GetComponentInChildren<AbilitiesManager>().Setup(source);
        Tab.Setup(source);
    }

    public void Cleanup()
    {
        EquipmentView.GetComponent<EquipmentViewMono>().Close();
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Close();
        InfoView.GetComponentInChildren<PlayerInfoMono>().Close();
        AbilitiesView.GetComponentInChildren<AbilitiesManager>().Close();
    }

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
    }
}
