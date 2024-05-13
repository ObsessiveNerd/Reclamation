using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Melee,
    Ranged
}

public class Weapon : MonoBehaviour
{
    public WeaponType WeaponType;
    public GameObject Effect;

    public void Attack(GameObject source, List<GameObject> targets)
    {
        SpawnEffect(source);
        var targetsCopy = new List<GameObject>(targets);
        foreach (GameObject target in targetsCopy)
            ApplyDamageTo(target);
    }

    public void Attack(GameObject source, GameObject target)
    {
        SpawnEffect(source);
        ApplyDamageTo(target);
    }

    void ApplyDamageTo(GameObject target)
    {
        var health = target?.GetComponent<Health>() ;
        if (health != null)
            health.TakeDamage(4, DamageType.None);
    }

    void SpawnEffect(GameObject source)
    {
        var instance = Instantiate(Effect, source.transform.position, source.transform.rotation);
        instance.AddComponent<DestroyAfter>().Begin(1f);
    }
}
