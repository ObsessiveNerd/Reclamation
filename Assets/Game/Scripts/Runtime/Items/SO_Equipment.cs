using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class SerializedEquipment : SerializedItem
{
    public List<Slot> ValidEquipSlots;
    public List<Resistance> Resistances;
    
    [SerializeReference, Subclass(IsList = true)]
    public List<Effects> OnEquipEffects;
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Reclaim/Equipment")]
public class SO_Equipment : SO_Item
{
    [SerializeField]
    private SerializedEquipment SerializedEquipment;
    private List<AsyncOperationHandle> m_Handles = new List<AsyncOperationHandle>();

    public void Equip(GameObject source)
    {

    }

    public void Unequip(GameObject source)
    {

    }

    public override SerializedItem GetSerializedItem()
    {
        return SerializedEquipment;
    }

    public override string Serialize()
    {
        SerializedEquipment.IconName = Icon.name;
        SerializedEquipment.SpriteName = Sprite.name;

        return JsonUtility.ToJson(SerializedEquipment);
    }

    public override void Deserialize(string json)
    {
        SerializedEquipment = JsonUtility.FromJson<SerializedEquipment>(json);

        var spriteHandle = Addressables.LoadAssetAsync<Sprite>(SerializedEquipment.SpriteName);
        Sprite = spriteHandle.WaitForCompletion();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(SerializedEquipment.IconName);
        Icon = iconHandle.WaitForCompletion();

        m_Handles.Add(spriteHandle);
        m_Handles.Add(iconHandle);
    }

    private void OnDestroy()
    {
        foreach (var handle in m_Handles)
            Addressables.Release(handle);
    }
}
