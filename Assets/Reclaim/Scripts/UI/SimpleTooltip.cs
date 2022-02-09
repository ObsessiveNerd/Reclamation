using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Title;
    public string Body;

    private GameObject m_Popup;
    private GameObject m_PopupInstance;

    // Start is called before the first frame update
    void Start()
    {
        m_Popup = Resources.Load<GameObject>("Prefabs/UI/ItemPopup");
    }

    void OnDisable()
    {
        Destroy(m_PopupInstance);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(m_PopupInstance);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_PopupInstance = Instantiate(m_Popup);
        m_PopupInstance.GetComponent<InfoMono>().SetData(Title, Body);
        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
    }
}
