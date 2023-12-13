using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectorMono : UpdatableUI//, IUpdatableUI
{
    public GameObject SpellObject;
    public GameObject SpellView;

    public override void UpdateUI()
    {
        Close();
        GameObject source = Services.PlayerManagerService.GetActivePlayer();
        if (source == null)
            return;

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                    .With(EventParameter.Abilities, new List<GameObject>());

        var spellList = source.FireEvent(getSpells).GetValue<List<GameObject>>(EventParameter.Abilities);
        getSpells.Release();

        if (spellList.Count == 0)
        { 
            SpellObject.SetActive(false);
            return;
        }

        int index = 1;
        foreach (GameObject spell in spellList)
        {
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
                spriteGo.transform.SetParent(SpellView.transform, false);

                spriteGo.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();

                GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);
                source.FireEvent(getMana);

                GameEvent getSpellCost = GameEventPool.Get(GameEventId.ManaCost)
                                        .With(EventParameter.Value, 0);
                spell.FireEvent(getSpellCost);

                if (getMana.GetValue<int>(EventParameter.Value) < getSpellCost.GetValue<int>(EventParameter.Value))
                    spriteRenderer.color = Color.black;
                else
                    spriteRenderer.color = Color.white;

                getSpellCost.Release();
                getMana.Release();

                index++;
            }
        }
    }

    protected override void OnDisable()
    {
        Close();
        base.OnDisable();
    }

    void Close()
    {
        foreach (Transform go in SpellView.GetComponentsInChildren<Transform>())
            if(SpellView.transform != go)
                Destroy(go.gameObject);
    }
}
