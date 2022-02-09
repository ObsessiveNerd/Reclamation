using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemMono : ItemMono, IPointerClickHandler
{
    public IEntity Source { get; set; }
    public IEntity ItemObject { get; set; }

    public void Init(IEntity source, IEntity thisObject)
    {
        Source = source;
        ItemObject = thisObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Source == null || ItemObject == null)
                return;

            OnClick();
        }
    }

    protected virtual void OnClick()
    {
        
        GameEvent getContextMenuActions = GameEventPool.Get(GameEventId.GetContextMenuActions)
            .With(EventParameters.Entity, Source.ID)
            .With(EventParameters.InventoryContextActions , new List<ContextMenuButton>());

        var result = ItemObject.FireEvent(getContextMenuActions).GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions );
        getContextMenuActions.Release();

        var contextMenu = ContextMenuMono.CreateNewContextMenu(); //FindObjectOfType<ContextMenuMono>();
        var cmm = contextMenu.GetComponent<ContextMenuMono>();

        foreach (var action in result)
            cmm.AddButton(action, Source);
    }

    protected override string GetItemId()
    {
        return ItemObject?.ID;
    }
}
