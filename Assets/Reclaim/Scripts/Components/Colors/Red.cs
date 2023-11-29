using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Red : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AlterSprite)
        {
            var renderer = gameEvent.GetValue<SpriteRenderer>(EventParameter.Renderer);
            renderer.color = Color.red;
        }
    }
}

public class DTO_Red : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Red();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Red);
    }
}
