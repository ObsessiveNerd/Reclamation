using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Component
{
    public BodyPart PreferredBodyPartWhenEquipped;

    public Equipment(BodyPart preferredBodyPart)
    {
        PreferredBodyPartWhenEquipped = preferredBodyPart;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.TryEquip);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        base.HandleEvent(gameEvent);

        if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            EventBuilder amIEquiped = EventBuilderPool.Get(GameEventId.CheckItemEquiped)
                                        .With(EventParameters.Item, Self.ID)
                                        .With(EventParameters.Value, false);

            bool isEquiped = source.FireEvent(amIEquiped.CreateEvent()).GetValue<bool>(EventParameters.Value);

            if(isEquiped)
            {
                ContextMenuButton button = new ContextMenuButton("Unequip", () =>
                {
                    EventBuilder unequip = EventBuilderPool.Get(GameEventId.Unequip)
                                .With(EventParameters.Entity, source.ID)
                                .With(EventParameters.Item, Self.ID);

                    source.FireEvent(unequip.CreateEvent(), true);
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
            }
            else
            {
                ContextMenuButton button = new ContextMenuButton("Equip", () =>
                {
                    EventBuilder equip = EventBuilderPool.Get(GameEventId.Equip)
                                            .With(EventParameters.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameters.Equipment, Self.ID);

                    source.FireEvent(equip.CreateEvent(), true);
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
            }
        }

        if (gameEvent.ID == GameEventId.TryEquip)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            EventBuilder equip = EventBuilderPool.Get(GameEventId.Equip)
                                            .With(EventParameters.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameters.Equipment, Self.ID);
            Debug.LogWarning($"{source?.Name} is trying to equip {Self?.Name}");
            source.FireEvent(equip.CreateEvent(), true);
        }
    }
}

public class DTO_Equipment : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (data.Contains("="))
            value = data.Split('=')[1];
        BodyPart bp = (BodyPart)Enum.Parse(typeof(BodyPart), value);
        Component = new Equipment(bp);
    }

    public string CreateSerializableData(IComponent component)
    {
        Equipment equipment = (Equipment)component;
        return $"{nameof(Equipment)}: {equipment.PreferredBodyPartWhenEquipped.ToString()}";
    }
}
