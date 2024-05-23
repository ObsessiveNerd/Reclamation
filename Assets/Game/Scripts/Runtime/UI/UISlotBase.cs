using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;


public class UISlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    protected SO_Item Item;

    public Image Icon;
    protected ContextMenu m_ContextMenu;
    protected Tooltip instance;

    protected GameObject m_Source;

    // Start is called before the first frame update
    void Start()
    {
        this.m_ContextMenu = WidgetUtility.Find<ContextMenu>("ContextMenu");
        instance = WidgetUtility.Find<Tooltip>("Tooltip");

        if (Item != null)
            SetItem(FindFirstObjectByType<PlayerInputController>().gameObject, Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        instance.Close();
    }

    public void SetItem(GameObject source, SO_Item item)
    {
        m_Source = source;
        Item = item;
        Icon.sprite = item.Icon;
        Icon.color = new UnityEngine.Color(Icon.color.r, Icon.color.g, Icon.color.b, 255f);
    }

    public void Clear()
    {
        m_Source = null;
        Item = null;
        Icon.sprite = null;
        Icon.color = new UnityEngine.Color(Icon.color.r, Icon.color.g, Icon.color.b, 0f);
        instance.Close();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item != null)
        {
            instance.Show(WidgetUtility.ColorString(Item.name, UnityEngine.Color.white),
                WidgetUtility.ColorString(Item.GetDescription(), UnityEngine.Color.white), null, null, 300, true);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {

    }
}
