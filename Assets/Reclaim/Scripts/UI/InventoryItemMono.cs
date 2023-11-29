using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemMono : ItemMono, IPointerClickHandler
{
    public IEntity Source { get; set; }
    public IEntity ItemObject { get; set; }

    public GameObject StackableView;
    public TextMeshProUGUI ItemNumberText;
    public bool AllowConxtMenuOptions = true;

    public void Init(IEntity source, IEntity thisObject)
    {
        Source = source;
        ItemObject = thisObject;

        if (ItemObject.HasComponent(typeof(Stackable)))
        {
            StackableView.SetActive(true);
            var count = 1;
            if (Source.HasComponent(typeof(Inventory)))
                count = Source.GetComponent<Inventory>().InventoryItems.Where(item => item.Name == ItemObject.Name).Count();
            else if (Source.HasComponent(typeof(ItemContainer)))
                count = Source.GetComponent<ItemContainer>().IDToEntityMap.Values.Where(e => e.Name == ItemObject.Name).Count();

            ItemNumberText.text = $"x{count}";
        }
        else
            StackableView.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (eventData.button == PointerEventData.InputButton.Left && AllowConxtMenuOptions)
        {
            if (Source == null || ItemObject == null)
                return;

            OnClick();
        }
    }

    protected virtual void OnClick()
    {
        
        GameEvent getContextMenuActions = GameEventPool.Get(GameEventId.GetContextMenuActions)
            .With(EventParameter.Entity, Source.ID)
            .With(EventParameter.InventoryContextActions , new List<ContextMenuButton>());

        var result = ItemObject.FireEvent(getContextMenuActions).GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions );
        getContextMenuActions.Release();

        var contextMenu = ContextMenuMono.CreateNewContextMenu();
        var cmm = contextMenu.GetComponent<ContextMenuMono>();

        foreach (var action in result)
            cmm.AddButton(action, Source);
    }

    protected override string GetItemId()
    {
        return ItemObject?.ID;
    }
}
