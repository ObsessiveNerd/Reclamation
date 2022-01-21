using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeTextMono : MonoBehaviour
{
    float m_DeltaTime = 0f;
    float m_StartTime;
    float m_Lifetime = 0f;
    IEntity m_Target;

    public void Setup(string text, float lifetime, IEntity target, Color color)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = color;

        m_Lifetime = lifetime;
        m_StartTime = Time.time;
        //transform.position = startPos;
        m_Target = target;

        GameObject go = WorldUtility.GetGameObject(m_Target);
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(go.transform.position);
        newPos.y += (go.GetComponent<SpriteRenderer>().sprite.textureRect.height);
        transform.position = newPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        return;

        if (m_Target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        GameObject go = WorldUtility.GetGameObject(m_Target);
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(go.transform.position);
        newPos.y += (go.GetComponent<SpriteRenderer>().sprite.textureRect.height);
        newPos.y += (Time.time - m_StartTime) * 2;
        transform.position = newPos;

        if (m_DeltaTime > m_Lifetime)
            Destroy(gameObject);
        else
            m_DeltaTime += Time.deltaTime;
    }
}
