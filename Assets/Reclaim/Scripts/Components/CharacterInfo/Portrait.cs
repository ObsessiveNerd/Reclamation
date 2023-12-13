using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : EntityComponent
{
    public Sprite Sprite;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetPortrait, GetPortrait);
    }

    void GetPortrait(GameEvent gameEvent)
    {
         if(Sprite == null)
                Debug.Log("Sprite null");
            gameEvent.Paramters[EventParameter.RenderSprite] = Sprite;
    }
}