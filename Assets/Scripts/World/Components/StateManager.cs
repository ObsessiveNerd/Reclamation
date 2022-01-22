using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : GameService
{
    public void GameOver(bool win)
    {
        GameObject.FindObjectOfType<GameEndMono>().EnableEndState(win);
    }
}
