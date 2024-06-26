using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    private CircleCollider2D m_VisionCollider;
    private List<GameObject> m_Targets = new List<GameObject>();

    private void Start()
    {
        m_VisionCollider = GetComponent<CircleCollider2D>();
    }

    public float GetVisualRange()
    {
        return m_VisionCollider.radius;
    }



    public void SetVision(float radius) 
    {
        m_VisionCollider.radius = radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
            m_Targets.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(m_Targets.Contains(collision.gameObject))
            m_Targets.Remove(collision.gameObject);
    }

    public GameObject GetTarget()
    {
        if(m_Targets.Count > 0)
            return m_Targets[0];
        return null;
    }
}
