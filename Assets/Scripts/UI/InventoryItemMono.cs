using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemMono : MonoBehaviour, IPointerClickHandler
{
    IEntity m_Source { get; set; }
    IEntity m_Object { get; set; }

    public void Init(IEntity source, IEntity thisObject)
    {
        m_Source = source;
        m_Object = thisObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_Source == null || m_Object == null)
                return;

            EventBuilder getContextMenuActions = new EventBuilder(GameEventId.GetContextMenuActions)
                                    .With(EventParameters.Entity, m_Source.ID)
                                    .With(EventParameters.InventoryContextActions , new List<ContextMenuButton>());

            var result = m_Object.FireEvent(getContextMenuActions.CreateEvent()).GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions );

            var contextMenu = FindObjectOfType<ContextMenuMono>();

            foreach (var action in result)
                contextMenu.AddButton(action);
        }
    }
}
