using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    GameObject ActionBar;
    public Transform Content;
    public GameObject NoAbilitiesText;

    IEntity m_Source;

    public void Setup(IEntity source)
    {
        Close();
        ActionBar = FindObjectOfType<SpellSelectorMono>().gameObject;


        m_Source = source;

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                .With(EventParameters.SpellList, new HashSet<string>());

        var spellList = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameters.SpellList);
        getSpells.Release();

        if (spellList.Count == 0)
            NoAbilitiesText.SetActive(true);
        else
            NoAbilitiesText.SetActive(false);

        foreach (string spellId in spellList)
        {
            IEntity spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameters.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameters.RenderSprite);
            getSpriteEvent.Release();
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(Content);

                spriteGo.GetComponent<SpellAbilityUIMono>().HideIndex();
                spriteGo.GetComponent<SpellAbilityUIMono>().Setup(spell);
            }
        }
    }

    public void Close()
    {
        foreach (var ability in Content.GetComponentsInChildren<SpellAbilityUIMono>())
        {
            Destroy(ability.gameObject);
        }
    }

    public void UpdateUI(IEntity source)
    {
        Close();
        Setup(source);
    }

    //private void Update()
    //{
    //    if (m_Source == null)
    //        return;

    //    var abilities = Services.PlayerManagerService.GetPlayerActiveAbilities(m_Source.ID);
    //    if (abilities.Count == 0)
    //    {
    //        ActionBar.SetActive(false);
    //        return;
    //    }
    //    else
    //        ActionBar.SetActive(true);
    //}
}
