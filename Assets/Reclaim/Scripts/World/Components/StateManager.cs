using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : GameService
{
    public TimeProgression TimeProgress {get { return m_TimeProgression; }}

    public bool GameEnded = false;

    public void GameOver(bool win)
    {
        GameEnded = true;
        m_TimeProgression.Stop();
        GameObject.FindObjectOfType<GameEndMono>().EnableEndState(win);
        Services.SaveAndLoadService.CleanCurrentSave();
    }

    public void CleanGameObjects()
    {
        GameObject.FindObjectOfType<GameEndMono>().CleanGameObjects();
    }
}
