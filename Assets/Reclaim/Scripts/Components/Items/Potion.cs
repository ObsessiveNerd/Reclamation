using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.Quaff, Quaff);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions, GetContextMenuActions);
    }

    void Quaff(GameEvent gameEvent)
    {
        GameObject target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));

        GameEvent builder = GameEventPool.Get(GameEventId.ApplyEffectToTarget)
                                    .With(EventParameter.Entity, target);
        gameObject.FireEvent(gameObject, builder, true).Release();
    }

    void GetContextMenuActions(GameEvent gameEvent)
    {
        GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));

        ContextMenuButton button = new ContextMenuButton("Quaff", () =>
            {
                Debug.Log(source);
                GameEvent quaff = GameEventPool.Get(GameEventId.Quaff)
                                        .With(EventParameter.Entity, source);
                Services.PlayerManagerService.GetActivePlayer().FireEvent(quaff).Release();

                GameEvent remove = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameter.Item, gameObject);
                source.FireEvent(remove).Release();

                GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                        .With(EventParameter.SoundSource, source)
                                        .With(EventParameter.Key, SoundKey.Quaff);
                gameObject.FireEvent(playSound).Release();
            });

        gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(button);
    }
}