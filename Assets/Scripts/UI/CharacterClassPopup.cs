using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassPopup : MonoBehaviour
{
    CharacterCreationMono cc;
    private GameObject m_Popup;
    private GameObject m_PopupInstance;

    void Start()
    {
        m_Popup = Resources.Load<GameObject>("UI/ItemPopup");
        cc = GetComponentInParent<CharacterCreationMono>();
    }

    public void OnEnter()
    {
        m_PopupInstance = Instantiate(m_Popup);

        InfoMono info = m_PopupInstance.GetComponent<InfoMono>();
        info.SetData(cc.CharacterClass.text, cc.GetClassReadout());

        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void OnExit()
    {
        Destroy(m_PopupInstance);
        m_PopupInstance = null;
    }
}
