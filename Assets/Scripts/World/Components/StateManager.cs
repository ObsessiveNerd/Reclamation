using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GameWin);
        RegisteredEvents.Add(GameEventId.GameFailure);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GameWin)
            GameObject.FindObjectOfType<GameEndMono>().EnableEndState(true);

        if(gameEvent.ID == GameEventId.GameFailure)
            GameObject.FindObjectOfType<GameEndMono>().EnableEndState(false);
    }
}
