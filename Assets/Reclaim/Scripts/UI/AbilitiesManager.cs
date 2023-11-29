using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : UpdatableUI
{
    GameObject ActionBar;
    public Transform Content;
    public GameObject NoAbilitiesText;

    public override void UpdateUI()
    {
        IEntity source = Services.PlayerManagerService.GetActivePlayer();

        Close();
        ActionBar = FindObjectOfType<SpellSelectorMono>(true).gameObject;

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                .With(EventParameter.SpellList, new HashSet<string>());

        var spellList = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameter.SpellList);
        getSpells.Release();

        if (spellList.Count == 0)
            NoAbilitiesText.SetActive(true);
        else
            NoAbilitiesText.SetActive(false);

        foreach (string spellId in spellList)
        {
            IEntity spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("Prefabs/UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameter.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameter.RenderSprite);
            getSpriteEvent.Release();
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(Content);

                spriteGo.GetComponent<SpellAbilityUIMono>().HideIndex();
                spriteGo.GetComponent<SpellAbilityUIMono>().Setup(Services.PlayerManagerService.GetActivePlayer(), spell, true, false);
            }
        }
    }

    public void Close()
    {
        foreach (var ability in Content.GetComponentsInChildren<SpellAbilityUIMono>())
            Destroy(ability.gameObject);
    }
}
