using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static System.TimeZoneInfo;

public enum ProjectileType
{
    Arrow,
    Bolt,
    Rock
}

public class Projectile : EntityComponent
{
    public ProjectileType Type;
    Vector3 m_TargetPosition;

    void Start()
    {
        m_TargetPosition = transform.position;
    }

    public void Fire(GameObject target, List<Damage> baseDamageList)
    {
        m_TargetPosition = target.transform.position;
    }

    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, m_TargetPosition, 20f * Time.deltaTime);

        //if (Vector3.Distance(transform.position, m_TargetPosition) < 0.1f)
        //{
        //    NetworkObject.Despawn();
        //    Destroy(gameObject);
        //}
    }
} 
