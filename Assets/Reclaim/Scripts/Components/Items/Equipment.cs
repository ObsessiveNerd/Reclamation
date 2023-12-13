using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : EntityComponent
{
    public BodyPart PreferredBodyPartWhenEquipped;

    public Equipment(BodyPart preferredBodyPart)
    {
        PreferredBodyPartWhenEquipped = preferredBodyPart;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.TryEquip);
        RegisteredEvents.Add(GameEventId.GetBodyPartType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        base.HandleEvent(gameEvent);

        if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            GameEvent amIEquiped = GameEventPool.Get(GameEventId.CheckItemEquiped)
                                        .With(EventParameter.Item, Self.ID)
                                        .With(EventParameter.Value, false);

            bool isEquiped = source.FireEvent(amIEquiped).GetValue<bool>(EventParameter.Value);
            amIEquiped.Release();

            if(isEquiped)
            {
                ContextMenuButton button = new ContextMenuButton("Unequip", () =>
                {
                    GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                .With(EventParameter.Entity, source.ID)
                                .With(EventParameter.Item, Self.ID);

                    source.FireEvent(unequip, true).Release();
                    Services.WorldUIService.UpdateUI();
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(button);
            }
            else
            {
                ContextMenuButton button = new ContextMenuButton("Equip", () =>
                {
                    GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                                            .With(EventParameter.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameter.Equipment, Self.ID);

                    GameEvent remove = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameter.Item, Self.ID);

                    source.FireEvent(remove);

                    //foreach(var player in Services.WorldDataQuery.GetPlayableCharacters())
                    //    Services.EntityMapService.GetEntity(player).FireEvent(remove);

                    remove.Release();

                    Services.EntityMapService.GetEntity(Services.WorldDataQuery.GetActivePlayerId()).FireEvent(equip, true).Release();
                    Services.WorldUIService.UpdateUI();
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(button);
            }
        }

        else if (gameEvent.ID == GameEventId.TryEquip)
        {
            GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                                            .With(EventParameter.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameter.Equipment, Self.ID);
            Debug.LogWarning($"{source?.Name} is trying to equip {Self?.Name}");
            source.FireEvent(equip, true).Release();
        }

        else if (gameEvent.ID == GameEventId.GetBodyPartType)
        {
            gameEvent.Paramters[EventParameter.BodyPart] = PreferredBodyPartWhenEquipped;
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
