using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOT : EntityComponent
{
    public int RegenSpeed;

    int currentTurns = 0;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.EndTurn, EndTurn);
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void EndTurn(GameEvent gameEvent)
    {
        GameObject target = gameEvent.HasParameter(EventParameter.Entity) ?
                Services.EntityMapService.GetEntity(gameEvent.GetValue<GameObject>(EventParameter.Entity)) : gameEvent;

        currentTurns++;
        if (currentTurns >= RegenSpeed)
        {
            CombatUtility.Attack(gameObject, target, gameObject);
            currentTurns = 0;
        }
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(DOT)}{Guid.NewGuid()}", $"Take damage every {RegenSpeed} turns.");
    }
}