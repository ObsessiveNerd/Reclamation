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
        if (MainHand != null)
            MainHand.GetComponent<EntityBehavior>().Activate();
        if (OffHand != null)
            OffHand.GetComponent<EntityBehavior>().Activate();
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
