using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class SerializedWeapon : SerializedItem
{
    [HideInInspector]
    public string EffectName;
    
    public List<Damage> Damage; 
    public List<EquipmentSlot> ValidEquipSlots;
    
    [SerializeReference, Subclass(IsList = true)]
    public List<Spell> Spells;
    
    [SerializeReference, Subclass(IsList = true)]
    public List<Effects> OnHitEffects;
    
    [SerializeReference, Subclass(IsList = true)]
    public List<Effects> OnEquipEffects;
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Reclaim/Weapon")]
public class SO_Weapon : SO_Item
{
    public GameObject Effect;

    [SerializeField]
    private SerializedWeapon Weapon;
    private List<AsyncOperationHandle> m_Handles = new List<AsyncOperationHandle>();

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

    public void OnEquip(GameObject target)
    {

    }

    public void OnUnequip(GameObject source)
    {

    }

    void ApplyDamageTo(GameObject target)
    {
        var health = target?.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(4, DamageType.None);
    }

    void SpawnEffect(GameObject source)
    {
        var instance = Instantiate(Effect, source.transform.position, source.transform.rotation);
        instance.AddComponent<DestroyAfter>().Begin(1f);
    }

    public override string Serialize()
    {
        Weapon.EffectName = Effect.name;
        Weapon.IconName = Icon.name;
        Weapon.SpriteName = Sprite.name;

        return JsonUtility.ToJson(Weapon);
    }

    public override void Deserialize(string json)
    {
        Weapon = JsonUtility.FromJson<SerializedWeapon>(json);
        
        var spriteHandle = Addressables.LoadAssetAsync<Sprite>(Weapon.SpriteName);
        Sprite = spriteHandle.WaitForCompletion();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Weapon.IconName);
        Icon = iconHandle.WaitForCompletion();


        var effectHandle = Addressables.LoadAssetAsync<GameObject>(Weapon.EffectName);
        Effect = effectHandle.WaitForCompletion();

        m_Handles.Add(spriteHandle);
        m_Handles.Add(iconHandle);
        m_Handles.Add(effectHandle);
    }

    public override SerializedItem GetSerializedItem()
    {
        return Weapon;
    }

    private void OnDestroy()
    {
        foreach (var handle in m_Handles)
            Addressables.Release(handle);
    }
}
