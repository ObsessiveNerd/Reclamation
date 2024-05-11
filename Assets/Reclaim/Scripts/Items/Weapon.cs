using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        foreach (GameObject target in targets)
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
            health.TakeDamage(0, DamageType.None);
    }

    void SpawnEffect(GameObject source)
    {
        var instance = Instantiate(Effect, source.transform.position, source.transform.rotation);
        instance.AddComponent<DestroyAfter>().Begin(1f);
    }
}
