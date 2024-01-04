using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentItemMono : MonoBehaviour, IPointerClickHandler
{
    GameObject m_Source;
    GameObject m_Item;
    Action m_Callback;

    public void Init(GameObject source, GameObject item, Action callback)
    {
        m_Source = source;
        m_Item = item;
        m_Callback = callback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
        //                        .With(EventParameters.Entity, m_Source.ID)
        //                        .With(EventParameters.Item, m_Item.ID);

        //m_Source.FireEvent(unequip.CreateEvent());
        //m_Callback?.Invoke();
    }
}
