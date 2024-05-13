using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    SpriteRenderer m_SpriteRenderer;
    EquipmentHandler m_EquipmentHandler;
    bool m_IsFlickering = false;

    // Start is called before the first frame update
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_EquipmentHandler = GetComponent<EquipmentHandler>();
    }

    public void TakeDamage(int damage, DamageType type)
    {
        float percent = m_EquipmentHandler.GetResistances(type);
        int adjustedDamage = (int)Mathf.Max(1, (percent / 100) * damage);

        CurrentHealth -= adjustedDamage;
        if(CurrentHealth <= 0 ) 
        {
            //die
            GetComponent<NetworkObject>().Despawn();
        }
        else
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
