using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;

public enum AttackType
{
    Melee,
    Ranged
}

[Serializable]
public class SerializedWeapon : SerializedItem
{
    [HideInInspector]
    public string EffectName;

    public List<Damage> Damage;
    public List<Slot> ValidEquipSlots;

    [SerializeReference, Subclass(IsList = true)]
    public List<Spell> Spells;

    [SerializeReference, Subclass(IsList = true)]
    public List<Effects> OnHitEffects;

    public AttackType AttackType;
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Reclaim/Weapon")]
public class SO_Weapon : SO_Item
{
    public GameObject Effect;

    [SerializeField]
    private SerializedWeapon Weapon;
    private List<AsyncOperationHandle> m_Handles = new List<AsyncOperationHandle>();

    //Used for melee attacks only
    public void Attack(GameObject source, MeleeArea meleeArea, Vector2 mousePosition)
    {
        if (Weapon.AttackType == AttackType.Melee)
        {
            var targets = meleeArea.ObjectsInRange;
            SpawnEffect(meleeArea.gameObject); 
            var targetsCopy = new List<GameObject>(targets);
            foreach (GameObject target in targetsCopy)
                ApplyDamageTo(target);
        }
        else
        { 
            //Skillshot attacks
        }
    }

    public void OnEquip(GameObject target)
    {

    }

    public void OnUnequip(GameObject source)
    {

    }

    public override string GetDescription()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Damage:");

        foreach (var damage in Weapon.Damage)
            sb.AppendLine($"{damage.DamageAmount} {damage.DamageType}");

        return sb.ToString();
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
