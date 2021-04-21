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
            EventBuilder dropItem = new EventBuilder(GameEventId.Drop)
                                    .With(EventParameters.Entity, m_Source.ID);

            m_Object.FireEvent(dropItem.CreateEvent());

            Destroy(gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }
}
