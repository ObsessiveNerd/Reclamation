using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Defense : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.TakeDamage, TakeDamage);
    }

    void TakeDamage(GameEvent gameEvent)
    {
        
    }
}