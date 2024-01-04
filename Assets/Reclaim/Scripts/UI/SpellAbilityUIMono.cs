using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellAbilityUIMono : ItemMono, IPointerDownHandler
{
    public GameObject BG;
    public GameObject Frame;
    public Image Image;

    protected override bool CanDrag => false;

    bool m_CanSetActiveAbilities = true;

    GameObject m_Source;
    GameObject m_Spell;

    public void Setup(GameObject source, GameObject spell, bool canSetAsActiveAbility, bool showIndex)
    {
        m_CanSetActiveAbilities = canSetAsActiveAbility;
        if (showIndex)
            ShowIndex();
        else
            HideIndex();

        m_Source = source;
        m_Spell = spell;
        GameEvent getActiveAbilities = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                            .With(EventParameter.Abilities, new List<GameObject>());
        m_Source.FireEvent(getActiveAbilities);
        List<GameObject> activeAbilities = getActiveAbilities.GetValue<List<GameObject>>(EventParameter.Abilities);
        if (!activeAbilities.Contains(m_Spell))
            Image.color = Color.white;
        else
            Image.color = Color.green;
        getActiveAbilities.Release();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (!m_CanSetActiveAbilities)
        //    return;

        //if(m_Source == null)
        //{
        //    string spellName = m_Spell == null ? "Null spell" : m_Spell.Name;
        //    Debug.LogError($"{spellName} has a null source.  Unable to add/remove from abilities");
        //    return;
        //}

        //if (m_Spell == null)
        //{
        //    Debug.LogError("A spell in SpellAbilityMono was null.");
        //    return;
        //}


        //if (eventData.clickCount == 1)
        //{
        //    Debug.Log("Double clicked");
        //    GameEvent activeAbility = GameEventPool.Get(GameEventId.AddToActiveAbilities)
        //                                .With(EventParameter.Entity, m_Spell.ID);

        //    m_Source.FireEvent(activeAbility).Release();
        //    GameEvent getSpells = GameEventPool.Get(GameEventId.GetActiveAbilities)
        //                            .With(EventParameter.Abilities, new List<GameObject>());

        //    var spellList = m_Source.FireEvent(getSpells).GetValue<List<GameObject>>(EventParameter.Abilities);
        //    getSpells.Release();

        //    if (spellList.Count == 0)
        //    {
        //        FindObjectOfType<SpellSelectorMono>(true).gameObject.SetActive(false);
        //    }
        //    else
        //        FindObjectOfType<SpellSelectorMono>(true).gameObject.SetActive(true);

        //    Services.WorldUIService.UpdateUI();
        //}
    }

    public void HideIndex()
    {
        BG.SetActive(false);
        Frame.SetActive(false);
    }
    public void ShowIndex()
    {
        BG.SetActive(true);
        Frame.SetActive(true);
    }

    protected override string GetItemId()
    {
        return "";
        //return m_Spell?.ID;
    }
}
