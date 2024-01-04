using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReferences : MonoBehaviour
{
    public GameObject SpellExaminer;
    public GameObject SpellSelector;
    public GameObject CharacterManager;
    public GameObject Chest;
    public GameObject EnchantmentManager;
    public GameObject EndState;
    public GameObject DebugMenu;
    public GameObject SelectFromAllSpells;

    public void OpenSpellExaminer(List<string> spellIds)
    {
        SpellExaminer.SetActive(true);
        SpellExaminer.GetComponent<SpellExaminationUI>().Setup(spellIds);
    }

    public void OpenSelectFromAllSpells()
    {
        SelectFromAllSpells.SetActive(true);
    }

    public void OpenCharacterManager()
    {
        if (CharacterManager.activeInHierarchy)
            return;

        CharacterManager.SetActive(true);
        CharacterManager.GetComponent<CharacterManagerMono>().Setup();
    }

    public void OnOpenDebugMenu()
    {
        DebugMenu.SetActive(true);
    }

    public void OpenSpellSelector()
    {
        SpellSelector.SetActive(true);
    }

    public void OpenChest(GameObject chest)
    {
        Chest.SetActive(true);
        Chest.GetComponent<ChestMono>().Setup(chest);
    }

    //public void OpenEnchanter(GameObject source, GameObject enchantment)
    //{
    //    //EnchantmentManager.SetActive(true);
    //    //EnchantmentManager.GetComponent<EnchantmentManagerMono>().Setup(source, enchantment);
    //}
}
