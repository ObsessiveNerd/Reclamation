using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Component
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

            EventBuilder builder = new EventBuilder(GameEventId.ApplyEffectToTarget)
                                    .With(EventParameters.Entity, target.ID);
            FireEvent(Self, builder.CreateEvent());
        }

        else if(gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            ContextMenuButton button = new ContextMenuButton("Quaff", () =>
            {
                IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));

                EventBuilder quaff = new EventBuilder(GameEventId.Quaff)
                                        .With(EventParameters.Entity, source.ID);
                Self.FireEvent(quaff.CreateEvent());

                EventBuilder remove = new EventBuilder(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Entity, Self.ID);
                source.FireEvent(remove.CreateEvent());
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
