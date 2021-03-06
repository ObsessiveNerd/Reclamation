using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIController : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UIInput);
        RegisteredEvents.Add(GameEventId.CloseUI);
        RegisteredEvents.Add(GameEventId.OpenInventoryUI);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.OpenInventoryUI)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            List<IEntity> inventory = (List<IEntity>)gameEvent.Paramters[EventParameters.Value];
            GameObject.FindObjectOfType<InventoryMono>().Setup(source, inventory);
        }
    }
}
