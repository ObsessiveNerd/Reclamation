using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemMono : ItemMono, IPointerClickHandler
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

            EventBuilder getContextMenuActions = EventBuilderPool.Get(GameEventId.GetContextMenuActions)
                                    .With(EventParameters.Entity, m_Source.ID)
                                    .With(EventParameters.InventoryContextActions , new List<ContextMenuButton>());

            var result = m_Object.FireEvent(getContextMenuActions.CreateEvent()).GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions );


            var contextMenu = ContextMenuMono.CreateNewContextMenu(); //FindObjectOfType<ContextMenuMono>();
            var cmm = contextMenu.GetComponent<ContextMenuMono>();

            foreach (var action in result)
                cmm.AddButton(action, m_Source);
        }
    }

    protected override string GetItemId()
    {
        return m_Object?.ID;
    }
}
