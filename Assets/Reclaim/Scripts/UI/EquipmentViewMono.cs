using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentViewMono : UpdatableUI//, IUpdatableUI
{
    public Image CharacterSprite;
    public Sprite DefaultImage;

    public GameObject Head;
    public GameObject Torso;
    public GameObject LeftArm;
    public GameObject RightArm;
    public GameObject LeftLeg;
    //public GameObject RightLeg;
    public GameObject Back;
    public GameObject Necklace;
    public GameObject Ring1;
    public GameObject Ring2;

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
                //RightLeg,
                Back,
                Necklace,
                Ring1,
                Ring2
            };
        }
    }

    void SetEquipment(IEntity source, List<EntityComponent> components, List<GameObject> slots)
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
                for (int j = 0; j < slots[i].transform.childCount; j++)
                    if (slots[i].GetComponent<InventoryItemMono>() != null)
                        Destroy(slots[i].transform.GetChild(j).gameObject);
                    else
                        slots[i].transform.GetChild(j).gameObject.SetActive(false);

                UIUtility.CreateItemGameObject(source, equipment, slots[i].transform);
            }
            else
            {
                foreach (var inventoryItem in slots[i].GetComponentsInChildren<InventoryItemMono>(true))
                    Destroy(inventoryItem.gameObject);

                for (int j = 0; j < slots[i].transform.childCount; j++)
                    slots[i].transform.GetChild(j).gameObject.SetActive(true);
            }
        }
    }
    
    public override void UpdateUI()
    {
        var source = Services.PlayerManagerService.GetActivePlayer();

        foreach(var slotMono in AllEquipmentSlots)
            slotMono.GetComponent<EquipmentItemSlotMono>().Setup(source);

        GameEvent getEquipment = GameEventPool.Get(GameEventId.GetCurrentEquipment)
                                    .With(EventParameters.Head, null)
                                    .With(EventParameters.Torso, null)
                                    .With(EventParameters.Arms, null)
                                    .With(EventParameters.Legs, null)
                                    .With(EventParameters.Finger, null)
                                    .With(EventParameters.Neck, null)
                                    .With(EventParameters.Back, null);

        var firedEvent = source.FireEvent(getEquipment);

        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Head), new List<GameObject>() { Head });
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Torso), new List<GameObject>() { Torso });
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Arms), new List<GameObject>() { LeftArm, RightArm });
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Legs), new List<GameObject>() { LeftLeg/*, RightLeg */});
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Finger), new List<GameObject>() { Ring1, Ring2 });
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Neck), new List<GameObject>() { Necklace });
        SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameters.Back), new List<GameObject>() { Back });
        firedEvent.Release();

        GameEvent getPicture = GameEventPool.Get(GameEventId.GetPortrait)
                                .With(EventParameters.RenderSprite, null);
        source.FireEvent(getPicture);

        CharacterSprite.sprite = getPicture.GetValue<Sprite>(EventParameters.RenderSprite);
        getPicture.Release();
    }
}
