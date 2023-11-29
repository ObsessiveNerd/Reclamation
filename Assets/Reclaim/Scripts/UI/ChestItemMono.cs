using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestItemMono : ItemMono, IPointerClickHandler
{
    IEntity m_Chest { get; set; }
    IEntity m_Item { get; set; }
    IEntity m_Character { get; set; }

    public void Init(IEntity chest, IEntity item, IEntity character)
    {
        m_Chest = chest;
        m_Item = item;
        m_Character = character;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_Chest == null || m_Character == null || m_Item == null) 
                return;

            GameEvent remove = GameEventPool.Get(GameEventId.RemoveItem)
                                    .With(EventParameter.Item, m_Item.ID);
            m_Chest.FireEvent(remove).Release();

            GameEvent add = GameEventPool.Get(GameEventId.AddToInventory)
                                .With(EventParameter.Entity, m_Item.ID);
            m_Character.FireEvent(add).Release();
            DestroyPopup();
            //GameEvent getContextMenuActions = GameEventPool.Get(GameEventId.GetContextMenuActions)
            //                        .With(EventParameters.Entity, m_Source.ID)
            //                        .With(EventParameters.ChestContextActions , new List<ContextMenuButton>());

            //var result = m_Object.FireEvent(getContextMenuActions.CreateEvent()).GetValue<List<ContextMenuButton>>(EventParameters.ChestContextActions);

            //var contextMenu = FindObjectOfType<ContextMenuMono>();

            //foreach (var action in result)
            //    contextMenu.AddButton(action);
        }
    }

    protected override string GetItemId()
    {
        return m_Item?.ID;
    }
}
