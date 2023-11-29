using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : EntityComponent
{
    public string ArrowEntityName;

    public Bow(string ammoName)
    {
        ArrowEntityName = ammoName;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetAmmo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetAmmo)
            gameEvent.Paramters[EventParameter.Value] = EntityFactory.CreateEntity(ArrowEntityName).ID;
    }
}

public class DTO_Bow : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Bow(data.Split('=')[1]);
    }

    public string CreateSerializableData(IComponent component)
    {
        Bow bow = (Bow)component;
        return $"{nameof(Bow)}: {nameof(bow.ArrowEntityName)}={bow.ArrowEntityName}";
    }
}
