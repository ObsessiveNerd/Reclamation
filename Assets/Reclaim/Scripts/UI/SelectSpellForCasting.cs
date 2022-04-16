using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectSpellForCasting : ItemMono, IPointerDownHandler
{
    public Action SpellSelected;
    string m_Id;
    public void Init(string itemId)
    {
        m_Id = itemId;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SpellSelected?.Invoke();
    }

    protected override string GetItemId()
    {
        return m_Id;
    }
}
