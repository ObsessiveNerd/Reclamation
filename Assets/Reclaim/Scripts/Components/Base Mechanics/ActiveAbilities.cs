using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilities : EntityComponent
{
    public List<GameObject> ActiveAbilitiesList = new List<GameObject>();

    public ActiveAbilities(List<GameObject> entities)
    {
        ActiveAbilitiesList = entities;
    }

    public void Start()
    {
        //RegisteredEvents.Add(GameEventId.AddToActiveAbilities);
        //RegisteredEvents.Add(GameEventId.RemoveFromActiveAbilities);
        //RegisteredEvents.Add(GameEventId.GetActiveAbilities);
    }
}