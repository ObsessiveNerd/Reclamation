using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentBehavior : NetworkBehaviour
{
    public GameObject MainHand;
    public GameObject OffHand;

    Camera m_Camera;

    List<GameObject> m_ObjectsInRange = new List<GameObject>();
    NetworkObject m_ParentNetworkObject;

    void Awake()
    {
        if (MainHand != null)
            MainHand.GetComponent<EntityBehavior>().Activate();
        if (OffHand != null)
            OffHand.GetComponent<EntityBehavior>().Activate();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = FindFirstObjectByType<Camera>();
        m_ParentNetworkObject = GetComponentInParent<NetworkObject>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        //Might re-do this so the position doesn't have to be synced over the network
        //we could just transmit the actual damage data from the owner
        UpdatePositionServerRpc(m_Camera.ScreenToWorldPoint(Input.mousePosition), transform.parent.position);
    }

    [ServerRpc]
    void UpdatePositionServerRpc(Vector3 target, Vector3 source)
    {
        UpdatePositionClientRpc(target, source);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 target, Vector3 source)
    {
        var directionToAttack = (target - source).normalized;
        var postionOfAttack = (source + directionToAttack);
        postionOfAttack.z = 0f;

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = postionOfAttack;
    }

    public void Attack()
    {
        var attack = GameEventPool.Get(GameEventId.PerformAttack)
                        .With(EventParameter.Position, transform.position)
                        .With(EventParameter.Angle, transform.rotation.eulerAngles.z)
                        .With(EventParameter.DamageList, new List<DamageData>());
        MainHand.FireEvent(attack);

        var damageList = attack.GetValue<List<DamageData>>(EventParameter.DamageList);

        foreach (var damage in damageList)
        {
            int rolledDamage = damage.DamageAmount.Roll();
            GameEvent damageEvent = GameEventPool.Get(GameEventId.ApplyDamage)
                                    .With(EventParameter.DamageAmount, rolledDamage)
                                    .With(EventParameter.DamageType, damage.Type);

            var objsInRangeCpy = new List<GameObject>(m_ObjectsInRange);
            foreach (var obj in objsInRangeCpy)
                obj.FireEvent(damageEvent);

            damageEvent.Release();
        }

        attack.Release();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.tag != "Player")
            m_ObjectsInRange.Add(collisionObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (m_ObjectsInRange.Contains(collisionObject))
            m_ObjectsInRange.Remove(collisionObject);
    }
}
