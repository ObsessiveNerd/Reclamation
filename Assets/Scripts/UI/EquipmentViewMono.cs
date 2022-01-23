using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[Serializable]
//public class EquipmentSlotData
//{
//    public GameObject ObjectSlot;
//    public BodyPart BodyPartyType;
//}

public class EquipmentViewMono : MonoBehaviour//, IUpdatableUI
{
    public Sprite DefaultImage;

    public GameObject Head;
    public GameObject Torso;
    public GameObject LeftArm;
    public GameObject RightArm;
    public GameObject LeftLeg;

    //Needs to be hooked up
    public GameObject RightLeg;

    IEntity m_Source;

    List<GameObject> AllEquipmentSlots
    {
        get
        {
            return new List<GameObject>()
            {
                Head,
                Torso,
                LeftArm,
                RightArm,
                LeftArm,
                RightLeg
            };
        }
    }

    public void Setup(IEntity source)
    {
        if(source != null)
            m_Source = source;

        foreach(var slotMono in AllEquipmentSlots)
            slotMono.GetComponent<EquipmentItemSlotMono>().Setup(source);

        GameEvent getEquipment = GameEventPool.Get(GameEventId.GetCurrentEquipment)
                                    .With(EventParameters.Head, null)
                                    .With(EventParameters.Torso, null)
                                    .With(EventParameters.Arms, null)
                                    .With(EventParameters.Legs, null);

        var firedEvent = m_Source.FireEvent(getEquipment);

        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Head), new List<GameObject>() { Head });
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Torso), new List<GameObject>() { Torso });
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Arms), new List<GameObject>() { LeftArm, RightArm });
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Legs), new List<GameObject>() { LeftLeg });
        firedEvent.Release();
    }

    void SetEquipment(List<Component> components, List<GameObject> slots)
    {
        if (components == null || components.Count == 0)
            return;

        for(int i = 0; i < components.Count; i++)
        {
            if (i >= slots.Count)
                break;

            GameEvent builder = GameEventPool.Get(GameEventId.GetEquipment)
                                    .With(EventParameters.Equipment, "");
            components[i].HandleEvent(builder);

            string equipmentId = builder.GetValue<string>(EventParameters.Equipment);
            builder.Release();
            if (!string.IsNullOrEmpty(equipmentId))
            {
                IEntity equipment = EntityQuery.GetEntity(equipmentId);
                
                //TODO, we should probably not assume that the current equipment object is what we're trying to equip
                if (slots[i].transform.childCount > 0)
                    Destroy(slots[i].transform.GetChild(0).gameObject);

                UIUtility.CreateItemGameObject(m_Source, equipment, slots[i].transform);
            }
            else
            {
                if (slots[i].transform.childCount > 0)
                    Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void Cleanup()
    {
        
    }

    public void Close()
    {
        Cleanup();
    }

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        Setup(newSource);
    }
}
