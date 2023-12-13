using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purple : EntityComponent
{
    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AlterSprite)
        {
            var renderer = gameEvent.GetValue<SpriteRenderer>(EventParameter.Renderer);
            renderer.color = Color.magenta;
        }
    }
}

public class DTO_Purple : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Purple();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Purple);
    }
}
