using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Sharpness : Component
//{
//    public Sharpness()
//    {
//        RegisteredEvents.Add(GameEventId.AmAttacking);
//    }

//    public override void HandleEvent(GameEvent gameEvent)
//    {
//        GameEvent sharpness = new GameEvent(GameEventId.Sharpness, gameEvent.Paramters);
//        ((List<GameEvent>)gameEvent.Paramters[EventParameters.AdditionalGameEvents]).Add(sharpness);
//    }
//}

//public class DTO_Sharpness : IDataTransferComponent
//{
//    public IComponent Component { get; set; }

//    public void CreateComponent(string data)
//    {
//        Component = new Sharpness();
//    }

//    public string CreateSerializableData(IComponent component)
//    {
//        return nameof(Sharpness);
//    }
//}