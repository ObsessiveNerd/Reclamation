using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cyan : EntityComponent
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
            renderer.color = Color.cyan;
        }
    }
}

public class DTO_Cyan : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Cyan();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Cyan);
    }
}
