using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUpdate : GameService
{
    public bool WorldEnded = false;
    public bool StopTime = false;

    public void UpdateWorldView()
    {
        foreach (var tile in m_ChangedTiles)
            tile.UpdateTile();
        m_ChangedTiles.Clear();
    }

    public void ProgressUntilIdIsActive(string id)
    {
        m_TimeProgression.ProgressTimeUntilEntityHasTakenTurn(id);
    }

    public void ProgressTime()
    {
        if (!StopTime && !WorldEnded)
        {
            m_TimeProgression.Update();
            if (GameEventPool.GameEventsInUse)
                Debug.LogError("GameEvents were unreleased");
        }
    }
}
