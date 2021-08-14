using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoMono : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Info;

    RectTransform m_Rect;
    int m_Buffer = 20;

    public void SetData(string title, string info)
    {
        Title.text = title;
        Info.text = info;
    }

    private void Start()
    {
        m_Rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 pos = Input.mousePosition;
        pos.x += (m_Rect.rect.width / 2) + m_Buffer;

        if (pos.x + (m_Rect.rect.width / 2) > Screen.width)
            pos.x -= m_Rect.rect.width + (m_Buffer * 1.5f);

        if (pos.y + (m_Rect.rect.height/2) > Screen.height)
            pos.y -= (pos.y + (m_Rect.rect.height/2)) - Screen.height;

        if (pos.y - (m_Rect.rect.height / 2) < 0)
            pos.y -= 0 - (m_Rect.rect.height/2) + pos.y;

        transform.position = pos;
    }
}
