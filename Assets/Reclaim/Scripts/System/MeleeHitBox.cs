using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    public Transform Parent;
    public GameObject Weapon;

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
        //currently this damage is pulling from an event sent on the parent, which is routed through the body
        //we can rely solely on this for knowledage about what is equipped (change needed)
        var damageList = gameEvent.GetValue<List<DamageData>>(EventParameter.DamageList);

        foreach (var damage in damageList)
        {
            int rolledDamage = damage.DamageAmount.Roll();
            GameEvent damageEvent = GameEventPool.Get(GameEventId.ApplyDamage)
                                    .With(EventParameter.DamageAmount, rolledDamage)
                                    .With(EventParameter.DamageType, damage.Type);

            foreach (var obj in m_ObjectsInRange)
                obj.FireEvent(damageEvent);

            damageEvent.Release();
        }

        GameEvent spawnEffect = GameEventPool.Get(GameEventId.SpawnEffect)
                                .With(EventParameter.Position, transform.position)
                                .With(EventParameter.Angle, transform.rotation.eulerAngles.z);
        Weapon.FireEvent(spawnEffect);
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
