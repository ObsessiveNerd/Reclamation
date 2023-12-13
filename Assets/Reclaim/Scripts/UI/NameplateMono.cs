using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameplateMono : MonoBehaviour
{
    GameObject m_Target;

    public void Setup(GameObject entity)
    {
        m_Target = entity;
        GetComponent<TextMeshProUGUI>().text = m_Target?.Name;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_Target == null)
        {
            Destroy(gameObject);
            return;
        }

        GameObject go = WorldUtility.GetGameObject(m_Target);
        if(go == null)
        {
            Destroy(gameObject);
            return;
        }

        if (go.GetComponent<SpriteRenderer>().sprite != null)
        {
            Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(go.transform.position);
            newPos.y += (go.GetComponent<SpriteRenderer>().sprite.textureRect.height);
            transform.position = newPos;
        }
    }
}
