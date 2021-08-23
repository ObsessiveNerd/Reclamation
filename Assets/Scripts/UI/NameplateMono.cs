using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameplateMono : MonoBehaviour
{
    IEntity m_Target;

    public void Setup(IEntity entity)
    {
        m_Target = entity;
        GetComponent<TextMeshProUGUI>().text = m_Target?.Name;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_Target == null)
            return;

        GameObject go = WorldUtility.GetGameObject(m_Target);
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(go.transform.position);
        newPos.y += (go.GetComponent<SpriteRenderer>().sprite.textureRect.height);
        transform.position = newPos;
    }
}
