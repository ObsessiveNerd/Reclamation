using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    CanvasGroup canvasGroup;
    protected bool IsDragging;

    protected virtual bool CanDrag { get { return true; } }

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
     
        try
        { 
        previousParent = transform.parent.transform;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(CanDrag)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(CanDrag)
        {
            IsDragging = false;
            canvasGroup.blocksRaycasts = true;
            transform.SetParent(previousParent);
            transform.position = m_LastPosition;
            transform.SetAsLastSibling();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(CanDrag)
        {
            IsDragging = true;
            m_LastPosition = transform.position;
            transform.SetParent(FindFirstObjectByType<Canvas>().transform);
            transform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
        }
    }
}
