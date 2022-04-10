using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rage : EntityComponent
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
            var renderer = gameEvent.GetValue<SpriteRenderer>(EventParameters.Renderer);
            renderer.color = Color.red;
        }
    }
}

//public class DTO_Rage : IDataTransferComponent
//{
//    public IComponent Component { get; set; }

//    public void CreateComponent(string data)
//    {
//        Component = new Rage();
//    }

//    public string CreateSerializableData(IComponent component)
//    {
//        return nameof(Rage);
//    }
//}
