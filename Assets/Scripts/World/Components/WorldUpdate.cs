using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUpdate : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateWorldView);
        RegisteredEvents.Add(GameEventId.ProgressTime);
        RegisteredEvents.Add(GameEventId.PauseTime);
        RegisteredEvents.Add(GameEventId.UnPauseTime);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateWorldView)
        {
            foreach (var tile in m_Tiles)
                tile.Value.FireEvent(tile.Value, new GameEvent(GameEventId.UpdateTile));
        }

        if (gameEvent.ID == GameEventId.ProgressTime)
            ProgressTime();

        if (gameEvent.ID == GameEventId.PauseTime)
            StopTime = true;

        if (gameEvent.ID == GameEventId.UnPauseTime)
            StopTime = false;
    }

    public bool StopTime = false;

    public void ProgressTime()
    {
        if (!StopTime)
            m_TimeProgression.Update();
            //m_PlayerToTimeProgressionMap[m_ActivePlayer.Value].Update();
    }
}
