using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentViewMono : MonoBehaviour
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
        m_Source = source;
        EventBuilder getEquipment = new EventBuilder(GameEventId.GetCurrentEquipment)
                                    .With(EventParameters.Head, null)
                                    .With(EventParameters.Torso, null)
                                    .With(EventParameters.LeftArm, null)
                                    .With(EventParameters.RightArm, null)
                                    .With(EventParameters.Legs, null);

        var firedEvent = source.FireEvent(getEquipment.CreateEvent());

        SetEquipment(firedEvent.GetValue<string>(EventParameters.Head), Head);
        SetEquipment(firedEvent.GetValue<string>(EventParameters.Torso), Torso);
        SetEquipment(firedEvent.GetValue<string>(EventParameters.LeftArm), LeftArm);
        SetEquipment(firedEvent.GetValue<string>(EventParameters.RightArm), RightArm);
        SetEquipment(firedEvent.GetValue<string>(EventParameters.Legs), Legs);
    }

    void SetEquipment(string equipmentId, GameObject slot)
    {
        if (string.IsNullOrEmpty(equipmentId)) return;

        IEntity equipment = EntityQuery.GetEntity(equipmentId);
        EventBuilder getImage = new EventBuilder(GameEventId.GetInfo)
                                .With(EventParameters.RenderSprite, null);
        var equipmentInfo = equipment.FireEvent(getImage.CreateEvent());
        slot.GetComponent<Image>().sprite = equipmentInfo.GetValue<Sprite>(EventParameters.RenderSprite);
        var equipmentSlotMono = slot.GetComponentInParent<EquipmentItemMono>();
        if(equipmentSlotMono != null)
        {
            equipmentSlotMono.Init(m_Source, equipment, () =>
            {
                slot.GetComponent<Image>().sprite = DefaultImage;
            });
        }
    }

    public void Cleanup()
    {

    }
}
