using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeArea : NetworkBehaviour
{
    List<GameObject> m_ObjectsInRange = new List<GameObject>();
    public List<GameObject> ObjectsInRange
    {
        get
        {
            return m_ObjectsInRange;
        }
    }

    Camera m_Camera;
    Camera Camera
    {
        get
        {
            if (m_Camera == null)
                m_Camera = FindFirstObjectByType<Camera>();
            return m_Camera;
        }
    }
    void Start()
    {
        m_Camera = FindFirstObjectByType<Camera>();
    }

    public void ManualUpdate(Vector2 targetScreenPoint)
    {
        if (!IsOwner)
            return;

        if (Camera == null)
            return;

        Vector2 target = Camera.ScreenToWorldPoint(targetScreenPoint);
        Vector2 source = transform.parent.position;

        var directionToAttack = (target - source).normalized * 0.5f;
        var postionOfAttack = (source + directionToAttack);

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = postionOfAttack;

        UpdatePositionServerRpc(transform.position, transform.rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRpc(Vector2 pos, Quaternion rot)
    {
        UpdatePositionClientRpc(pos, rot);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector2 pos, Quaternion rot)
    {
        if(!IsOwner)
        {
            transform.rotation = rot;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.tag != transform.parent.gameObject.tag)
            m_ObjectsInRange.Add(collisionObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (m_ObjectsInRange.Contains(collisionObject))
            m_ObjectsInRange.Remove(collisionObject);
    }
}
