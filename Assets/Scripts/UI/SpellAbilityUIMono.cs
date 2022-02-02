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

    IEntity m_Source;
    IEntity m_Spell;

    public void Setup(/*IEntity source, */IEntity spell)
    {
        //m_Source = source;
        m_Spell = spell;
        GameEvent getActiveAbilities = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                            .With(EventParameters.Abilities, new List<IEntity>());
        Services.PlayerManagerService.GetActivePlayer().FireEvent(getActiveAbilities);
        List<IEntity> activeAbilities = getActiveAbilities.GetValue<List<IEntity>>(EventParameters.Abilities);
        if (!activeAbilities.Contains(m_Spell))
            Image.color = Color.white;
        else
            Image.color = Color.green;
        getActiveAbilities.Release();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("Double clicked");
            GameEvent activeAbility = GameEventPool.Get(GameEventId.AddToActiveAbilities)
                                        .With(EventParameters.Entity, m_Spell.ID);

            Services.PlayerManagerService.GetActivePlayer().FireEvent(activeAbility).Release();
            Services.WorldUIService.UpdateUI();
        }
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
        return m_Spell?.ID;
    }
}
