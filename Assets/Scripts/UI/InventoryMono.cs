using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryMono : EscapeableMono
{
    public GameObject InventoryObject;

    public GameObject InventoryView;
    public GameObject EquipmentView;
    public GameObject CharacterView;

    IEntity m_Source;
    public void Setup(IEntity source, List<IEntity> inventory)
    {
        m_Source = source;
        GameObject spriteGoResource = Resources.Load<GameObject>("UI/InventoryItem");
        foreach(var item in inventory)
        {
            Sprite sprite = item.FireEvent(item, new GameEvent(GameEventId.GetSprite, 
                new KeyValuePair<string, object>(EventParameters.RenderSprite, null))).GetValue<Sprite>(EventParameters.RenderSprite);
            if(sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(InventoryView.transform);
            }
        }

        InventoryObject.SetActive(true);
        EquipmentView.SetActive(true);
        CharacterView.SetActive(true);
    }

    protected override void OnEscape()
    {
        EquipmentView.SetActive(false);
        CharacterView.SetActive(false);
        InventoryObject.SetActive(false);
        foreach (Transform go in InventoryView.GetComponentsInChildren<Transform>())
            if(InventoryView.transform != go)
                Destroy(go.gameObject);
    }
}
