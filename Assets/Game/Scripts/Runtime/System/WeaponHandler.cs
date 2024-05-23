using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    public SO_Weapon MainHand;
    public SO_Weapon OffHand;

    List<GameObject> m_ObjectsInRange = new List<GameObject>();
    Camera m_Camera;
    void Start()
    {
        m_Camera = FindFirstObjectByType<Camera>();
    }

    public List<SO_Item> GetWeapons()
    {
        return new List<SO_Item>()
        {
            MainHand, OffHand
        };
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        Vector2 target = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 source = transform.parent.position;

        var directionToAttack = (target - source).normalized * 0.5f;
        var postionOfAttack = (source + directionToAttack);

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = postionOfAttack;
    }

    [ServerRpc]
    public void UpdatePositionServerRpc(Vector3 position)
    {
        UpdatePositionClientRpc(position);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    public void Attack()
    {
        if (MainHand != null)
        {
            var weapon = MainHand;
            if(weapon != null )
                weapon.Attack(gameObject, m_ObjectsInRange);
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
