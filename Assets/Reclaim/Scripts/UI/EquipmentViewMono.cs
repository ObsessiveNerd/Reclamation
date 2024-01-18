using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentViewMono : UpdatableUI//, IUpdatableUI
{
    public Image CharacterSprite;
    public Image CharacterSprite2;
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
                LeftLeg,
                Back,
                Necklace,
                Ring1,
                Ring2
            };
        }
    }

    //void SetEquipment(GameObject source, List<ComponentBehavior> components, List<GameObject> slots)
    //{
    //    if (components == null || components.Count == 0)
    //        return;

    //    for(int i = 0; i < components.Count; i++)
    //    {
    //        if (i >= slots.Count)
    //            break;

    //        GameEvent builder = GameEventPool.Get(GameEventId.GetEquipment)
    //                                .With(EventParameter.Equipment, "");
    //        components[i].HandleEvent(builder);

    //        string equipmentId = builder.GetValue<string>(EventParameter.Equipment);
    //        builder.Release();
    //        if (!string.IsNullOrEmpty(equipmentId))
    //        {
    //            GameObject equipment = EntityQuery.GetEntity(equipmentId);

    //            //TODO, we should probably not assume that the current equipment object is what we're trying to equip
    //            for (int j = 0; j < slots[i].transform.childCount; j++)
    //                if (slots[i].GetComponent<InventoryItemMono>() != null)
    //                    Destroy(slots[i].transform.GetChild(j).gameObject);
    //                else
    //                    slots[i].transform.GetChild(j).gameObject.SetActive(false);

    //            UIUtility.CreateItemGameObject(source, equipment, slots[i].transform);
    //        }
    //        else
    //        {
    //            foreach (var inventoryItem in slots[i].GetComponentsInChildren<InventoryItemMono>(true))
    //                Destroy(inventoryItem.gameObject);

    //            for (int j = 0; j < slots[i].transform.childCount; j++)
    //                slots[i].transform.GetChild(j).gameObject.SetActive(true);
    //        }
    //    }
    //}
    
    public override void UpdateUI()
    {
        //var source = Services.PlayerManagerService.GetActivePlayer();

        //foreach(var slotMono in AllEquipmentSlots)
        //    slotMono.GetComponent<EquipmentItemSlotMono>().Setup(source);

        //GameEvent getEquipment = GameEventPool.Get(GameEventId.GetCurrentEquipment)
        //                            .With(EventParameter.Head, null)
        //                            .With(EventParameter.Torso, null)
        //                            .With(EventParameter.Arms, null)
        //                            .With(EventParameter.Legs, null)
        //                            .With(EventParameter.Finger, null)
        //                            .With(EventParameter.Neck, null)
        //                            .With(EventParameter.Back, null);

        //var firedEvent = source.FireEvent(getEquipment);

        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Head), new List<GameObject>() { Head });
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Torso), new List<GameObject>() { Torso });
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Arms), new List<GameObject>() { LeftArm, RightArm });
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Legs), new List<GameObject>() { LeftLeg/*, RightLeg */});
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Finger), new List<GameObject>() { Ring1, Ring2 });
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Neck), new List<GameObject>() { Necklace });
        //SetEquipment(source, firedEvent.GetValue<List<EntityComponent>>(EventParameter.Back), new List<GameObject>() { Back });
        //firedEvent.Release();

        //GameEvent getPicture = GameEventPool.Get(GameEventId.GetPortrait)
        //                        .With(EventParameter.RenderSprite, null);
        //source.FireEvent(getPicture);

        //CharacterSprite.sprite = getPicture.GetValue<Sprite>(EventParameter.RenderSprite);
        //CharacterSprite2.sprite = getPicture.GetValue<Sprite>(EventParameter.RenderSprite);
        //getPicture.Release();
    }
}
