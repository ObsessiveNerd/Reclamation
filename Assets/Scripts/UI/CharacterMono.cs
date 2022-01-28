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

    public void Setup(IEntity source)
    {
        EquipmentView.GetComponent<EquipmentViewMono>().Setup(source);
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Setup(source);
        //CharacterStats.GetComponentInChildren<PlayerInfoMono>().Setup(source);

    }

    public void Cleanup()
    {
        EquipmentView.GetComponent<EquipmentViewMono>().Close();
        CharacterStats.GetComponentInChildren<CharacterStatsMono>().Close();
        //CharacterStats.GetComponentInChildren<PlayerInfoMono>().Close();
    }

    public void ToggleSelected(string toggleName)
    {
        switch(toggleName)
        {
            case "Equipment":
                CharacterStats.SetActive(false);
                EquipmentView.SetActive(true);
                InfoView.SetActive(false);
                break;
            case "Stats":
                CharacterStats.SetActive(true);
                EquipmentView.SetActive(false);
                InfoView.SetActive(false);
                break;
            case "Info":
                CharacterStats.SetActive(false);
                EquipmentView.SetActive(false);
                InfoView.SetActive(true);
                break;
        }
    }
}
