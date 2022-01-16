using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentViewMono : MonoBehaviour, IUpdatableUI
{
    public Sprite DefaultImage;

    public GameObject Head;
    public GameObject Torso;
    public GameObject LeftArm;
    public GameObject RightArm;
    public GameObject Legs;

    IEntity m_Source;

    public void Setup(IEntity source)
    {
        if(source != null)
            m_Source = source;

        WorldUtility.RegisterUI(this);
        EventBuilder getEquipment = EventBuilderPool.Get(GameEventId.GetCurrentEquipment)
                                    .With(EventParameters.Head, null)
                                    .With(EventParameters.Torso, null)
                                    .With(EventParameters.Arms, null)
                                    //.With(EventParameters.RightArm, null)
                                    .With(EventParameters.Legs, null);

        var firedEvent = m_Source.FireEvent(getEquipment.CreateEvent());

        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Head), new List<GameObject>() { Head });
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Torso), new List<GameObject>() { Torso });
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Arms), new List<GameObject>() { LeftArm, RightArm });
        //SetEquipment(firedEvent.GetValue<string>(EventParameters.RightArm), RightArm);
        SetEquipment(firedEvent.GetValue<List<Component>>(EventParameters.Legs), new List<GameObject>() { Legs });
    }

    void SetEquipment(List<Component> components, List<GameObject> slots)
    {
        //if (string.IsNullOrEmpty(equipmentId))
        //{
        //    slot.GetComponent<Image>().sprite = DefaultImage;
        //    return;
        //}

        if (components == null || components.Count == 0)
            return;

        for(int i = 0; i < components.Count; i++)
        {
            if (i >= slots.Count)
                break;

            EventBuilder builder = EventBuilderPool.Get(GameEventId.GetEquipment)
                                    .With(EventParameters.Equipment, "");
            var ge = builder.CreateEvent();
            components[i].HandleEvent(ge);

            string equipmentId = ge.GetValue<string>(EventParameters.Equipment);
            if (!string.IsNullOrEmpty(equipmentId))
            {
                IEntity equipment = EntityQuery.GetEntity(equipmentId);
                EventBuilder getImage = EventBuilderPool.Get(GameEventId.GetPortrait)
                                        .With(EventParameters.RenderSprite, null);
                var equipmentInfo = equipment.FireEvent(getImage.CreateEvent());
                slots[i].GetComponent<Image>().sprite = equipmentInfo.GetValue<Sprite>(EventParameters.RenderSprite);
                var equipmentSlotMono = slots[i].GetComponentInParent<InventoryItemMono>();
                if (equipmentSlotMono != null)
                    equipmentSlotMono.Init(m_Source, equipment);
            }
            else
            {
                slots[i].GetComponent<Image>().sprite = DefaultImage;
                slots[i].GetComponentInParent<InventoryItemMono>().Init(m_Source, null);
            }
        }
    }

    public void Cleanup()
    {
        
    }

    public void Close()
    {
        Cleanup();
        WorldUtility.UnRegisterUI(this);
    }

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        Setup(newSource);
    }
}
