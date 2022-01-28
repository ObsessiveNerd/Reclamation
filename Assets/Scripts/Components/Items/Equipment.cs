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
        RegisteredEvents.Add(GameEventId.GetBodyPartType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        base.HandleEvent(gameEvent);

        if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            GameEvent amIEquiped = GameEventPool.Get(GameEventId.CheckItemEquiped)
                                        .With(EventParameters.Item, Self.ID)
                                        .With(EventParameters.Value, false);

            bool isEquiped = source.FireEvent(amIEquiped).GetValue<bool>(EventParameters.Value);
            amIEquiped.Release();

            if(isEquiped)
            {
                ContextMenuButton button = new ContextMenuButton("Unequip", () =>
                {
                    GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                .With(EventParameters.Entity, source.ID)
                                .With(EventParameters.Item, Self.ID);

                    source.FireEvent(unequip, true).Release();
                    Services.WorldUIService.UpdateUI(Services.WorldDataQuery.GetActivePlayerId());
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
            }
            else
            {
                ContextMenuButton button = new ContextMenuButton("Equip", () =>
                {
                    GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                                            .With(EventParameters.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameters.Equipment, Self.ID);

                    GameEvent remove = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, Self.ID);

                    foreach(var player in Services.WorldDataQuery.GetPlayableCharacters())
                        Services.EntityMapService.GetEntity(player).FireEvent(remove);

                    remove.Release();

                    Services.EntityMapService.GetEntity(Services.WorldDataQuery.GetActivePlayerId()).FireEvent(equip, true).Release();
                    Services.WorldUIService.UpdateUI(Services.WorldDataQuery.GetActivePlayerId());
                });
                gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
            }
        }

        else if (gameEvent.ID == GameEventId.TryEquip)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                                            .With(EventParameters.EntityType, PreferredBodyPartWhenEquipped)
                                            .With(EventParameters.Equipment, Self.ID);
            Debug.LogWarning($"{source?.Name} is trying to equip {Self?.Name}");
            source.FireEvent(equip, true).Release();
        }

        else if (gameEvent.ID == GameEventId.GetBodyPartType)
        {
            gameEvent.Paramters[EventParameters.BodyPart] = PreferredBodyPartWhenEquipped;
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
