using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMono : DragAndDrop, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject m_Popup;
    private GameObject m_PopupInstance;

    protected override void Start()
    {
        m_Popup = Resources.Load<GameObject>("Prefabs/UI/ItemPopup");
        if (m_Popup == null)
            Debug.LogError("UI Item Popup could not be loaded.");
        base.Start();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(IsDragging)
        //    return;

        //Debug.Log("enter");
        //string id = GetItemId();
        //if (string.IsNullOrEmpty(id))
        //    return;

        //m_PopupInstance = Instantiate(m_Popup);
        //var itemInfo = m_PopupInstance.GetComponent<InfoMono>();

        //Dictionary<string, string> classToInfoMap = new Dictionary<string, string>();
        //GameEvent getInfo = GameEventPool.Get(GameEventId.GetInfo)
        //                        .With(EventParameter.Info, classToInfoMap);

        //var entity = EntityQuery.GetEntity(id);
        //entity.FireEvent(getInfo);
        //getInfo.Release();

        //List<string> sortedKeys = classToInfoMap.Keys.ToList();
        ////sortedKeys.Sort();

        //StringBuilder sb = new StringBuilder();
        //foreach (var key in sortedKeys)
        //    sb.AppendLine(classToInfoMap[key]);

        //itemInfo.SetData(entity.Name, sb.ToString());
        //m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    private void OnDisable()
    {
        DestroyPopup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyPopup();
    }

    protected void DestroyPopup()
    {
        Destroy(m_PopupInstance);
        m_PopupInstance = null;
    }

    protected virtual string GetItemId() { return ""; }
}
