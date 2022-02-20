using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Quaff);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Quaff)
        {
            IEntity target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));

            GameEvent builder = GameEventPool.Get(GameEventId.ApplyEffectToTarget)
                                    .With(EventParameters.Entity, target.ID);
            FireEvent(Self, builder).Release();
        }

        else if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));

            ContextMenuButton button = new ContextMenuButton("Quaff", () =>
            {
                Debug.Log(source);
                GameEvent quaff = GameEventPool.Get(GameEventId.Quaff)
                                        .With(EventParameters.Entity, source.ID);
                Self.FireEvent(quaff).Release();

                GameEvent remove = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, Self.ID);
                source.FireEvent(remove).Release();

                GameEvent playSound = GameEventPool.Get(GameEventId.Playsound)
                                        .With(EventParameters.SoundSource, source.ID)
                                        .With(EventParameters.Key, SoundKey.Quaff);
                Self.FireEvent(playSound).Release();
            });

            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
        }
    }
}

public class DTO_Potion : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Potion();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Potion);
    }
}
