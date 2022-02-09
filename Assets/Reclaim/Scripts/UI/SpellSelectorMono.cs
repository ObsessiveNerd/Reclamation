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
        IEntity source = Services.PlayerManagerService.GetActivePlayer();
        Close();
        GameEvent getSpells = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                    .With(EventParameters.Abilities, new List<IEntity>());

        var spellList = source.FireEvent(getSpells).GetValue<List<IEntity>>(EventParameters.Abilities);
        getSpells.Release();

        if (spellList.Count == 0)
        { 
            SpellObject.SetActive(false);
            return;
        }
        else
            SpellObject.SetActive(true);

        int index = 1;
        foreach (IEntity spell in spellList)
        {
            GameObject spriteGoResource = Resources.Load<GameObject>("Prefabs/UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameters.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameters.RenderSprite);
            getSpriteEvent.Release();
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(SpellView.transform);

                spriteGo.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();

                GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);
                source.FireEvent(getMana);

                GameEvent getSpellCost = GameEventPool.Get(GameEventId.ManaCost)
                                        .With(EventParameters.Value, 0);
                spell.FireEvent(getSpellCost);

                if (getMana.GetValue<int>(EventParameters.Value) < getSpellCost.GetValue<int>(EventParameters.Value))
                    spriteRenderer.color = Color.black;
                else
                    spriteRenderer.color = Color.white;

                getSpellCost.Release();
                getMana.Release();

                index++;
            }
        }
    }

    void Close()
    {
        foreach (Transform go in SpellView.GetComponentsInChildren<Transform>())
            if(SpellView.transform != go)
                Destroy(go.gameObject);
    }
}
