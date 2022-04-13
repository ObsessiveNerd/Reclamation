using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : GameService
{
    public TimeProgression TimeProgress {get { return m_TimeProgression; }}
    public void GameOver(bool win)
    {
        m_TimeProgression.Stop();
        GameObject.FindObjectOfType<GameEndMono>().EnableEndState(win);
    }
}
