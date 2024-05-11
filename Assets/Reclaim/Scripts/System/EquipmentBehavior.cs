using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentBehavior : NetworkBehaviour
{
    public GameObject MainHand;
    public GameObject OffHand;

    List<GameObject> m_ObjectsInRange = new List<GameObject>();

    void Awake()
    {
    }

    [ServerRpc]
    public void UpdatePositionServerRpc(Vector3 target, Vector3 source)
    {
        UpdatePositionClientRpc(target, source);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 target, Vector3 source)
    {
        var directionToAttack = (target - source).normalized * 0.7f;
        var postionOfAttack = (source + directionToAttack);
        postionOfAttack.z = 0f;

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = postionOfAttack;
    }

    public void Attack()
    {
        
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
