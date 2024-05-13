using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Reclaim/Weapon")]
public class SO_Weapon : ScriptableObject
{
    public string Name; 
    public GameObject Effect;
    public Sprite Icon;
    public Sprite Sprite;
    public List<Damage> Damage; 
    public List<EquipmentSlot> ValidEquipSlots;

    public void Attack(List<GameObject> targets)
    {

    }
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
