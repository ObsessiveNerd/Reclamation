using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    public Transform Parent;

    Camera m_Camera;
    BoxCollider2D m_Collider;

    List<GameObject> m_ObjectsInRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = FindFirstObjectByType<Camera>();
        m_Collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        var worldPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        var sourcePos = Parent.position;
        
        var directionToAttack = (worldPos - sourcePos).normalized;
        var postionOfAttack = (sourcePos + directionToAttack);
        postionOfAttack.z = 0f;

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = postionOfAttack;
    }

    public void Attack(GameEvent gameEvent)
    {
        foreach (var obj in m_ObjectsInRange)
        {
            Debug.Log(obj.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Add to trigger");
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.tag != "Player")
            m_ObjectsInRange.Add(collisionObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Remove from trigger");
        GameObject collisionObject = collision.gameObject;
        if(m_ObjectsInRange.Contains(collisionObject))
            m_ObjectsInRange.Remove(collisionObject);
    }
}
