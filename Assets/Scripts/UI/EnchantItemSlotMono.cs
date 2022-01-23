using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class EnchantItemSlotMono : MonoBehaviour, IDropHandler
{
    public bool AcceptsDrop;
    public void OnDrop(PointerEventData eventData)
    {
        if (AcceptsDrop)
        {
            Debug.Log($"{eventData.pointerDrag} is dropped");
            eventData.pointerDrag.GetComponent<DragAndDrop>().Set(transform.position, transform);
        }
    }
}
