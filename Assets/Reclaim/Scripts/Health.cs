using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    SpriteRenderer m_SpriteRenderer;
    bool m_IsFlickering = false;

    // Start is called before the first frame update
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, DamageType type)
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        if (m_IsFlickering)
            yield break;
        m_IsFlickering = true;
        var startColor = m_SpriteRenderer.color;

        for (int i = 0; i < 5; i++)
        {
            if(m_SpriteRenderer != null)
                m_SpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if(m_SpriteRenderer != null)
                m_SpriteRenderer.color = startColor;
            yield return new WaitForSeconds(0.1f);
        }
        m_IsFlickering= false;
    }
}
