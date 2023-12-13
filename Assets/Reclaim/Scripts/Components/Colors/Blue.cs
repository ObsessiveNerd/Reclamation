using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : EntityComponent
{
    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AlterSprite)
        {
            var renderer = gameEvent.GetValue<SpriteRenderer>(EventParameter.Renderer);
            renderer.color = Color.blue;
        }
    }
}

public class DTO_Blue : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Blue();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Blue);
    }
}
