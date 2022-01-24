using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    CanvasGroup canvasGroup;
    protected bool IsDragging;

    Transform previousParent;
    Vector2 m_LastPosition;

    public void Set(Vector2 position, Transform parent)
    {
        previousParent = parent;
        transform.SetParent(previousParent);
        m_LastPosition = position;
    }

    protected virtual void Start()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); //GetComponent<CanvasGroup>();
        m_LastPosition = transform.position;
        previousParent = transform.parent.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(previousParent);
        transform.position = m_LastPosition;
        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        m_LastPosition = transform.position;
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }
}
