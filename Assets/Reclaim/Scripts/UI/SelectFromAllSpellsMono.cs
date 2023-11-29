using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectFromAllSpellsMono : EscapeableMono//, IUpdatableUI
{
    public GameObject SpellObject;
    public GameObject SpellView;
    public GameObject NoSpellsText;

    protected override void OnEnable()
    {
        base.OnEnable();
        IEntity source = Services.PlayerManagerService.GetActivePlayer();
        if (source == null)
            return;

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                    .With(EventParameter.SpellList, new HashSet<string>());

        var spellList = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameter.SpellList);
        getSpells.Release();

        if (spellList.Count == 0)
        {
            //SpellObject.SetActive(false);
            NoSpellsText.SetActive(true);
            return;
        }

        NoSpellsText.SetActive(false);
        int index = 1;
        foreach (string spellId in spellList)
        {
            IEntity spell = Services.EntityMapService.GetEntity(spellId);
            GameObject spriteGoResource = Resources.Load<GameObject>("Prefabs/UI/SpellUILarge");
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

                spriteGo.GetComponent<SelectSpellForCasting>().Init(spellId);
                spriteGo.GetComponent<SelectSpellForCasting>().SpellSelected += () =>
                {
                    Services.PlayerManagerService.GetActivePlayer().RemoveComponent(typeof(InputControllerBase));
                    Services.PlayerManagerService.GetActivePlayer().AddComponent(new SpellcasterPlayerController(spell));
                    OnEscape();
                };

                getSpellCost.Release();
                getMana.Release();

                index++;
            }
        }
    }

    public override void OnEscape()
    {
        foreach (Transform go in SpellView.GetComponentsInChildren<Transform>())
            if(SpellView.transform != go)
                Destroy(go.gameObject);
        gameObject.SetActive(false);
    }
}
