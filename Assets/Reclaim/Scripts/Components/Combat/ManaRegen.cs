using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegen : EntityComponent
{
    public int RegenAmount;
    public int RegenSpeed;

    int currentTurns = 0;
    public ManaRegen(int regenAmount, int speed)
    {
        RegenAmount = regenAmount;
        RegenSpeed = speed;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.EndTurn);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
             GameObject target = gameEvent.HasParameter(EventParameter.Entity) ?
                Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity)) : Self;

            currentTurns++;
            if(currentTurns >= RegenSpeed)
            {
                GameEvent regenMana = GameEventPool.Get(GameEventId.RestoreMana)
                                            .With(EventParameter.Mana, RegenAmount);
                FireEvent(target, regenMana, true).Release();
                currentTurns = 0;
            }
        }

        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(ManaRegen)}{Guid.NewGuid()}", $"Renerate {RegenAmount} mana after {RegenSpeed} turns.");
        }
    }
}

public class DTO_ManaRegen : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int regen = 0;
        int speed = 0;

        var kvps = data.Split(',');
        foreach(var kvp in kvps)
        {
            var splitKvp = kvp.Split('=');
            var value = int.Parse(splitKvp[1]);
            if (splitKvp[0] == "RegenAmount")
                regen = value;
            else if (splitKvp[0] == "RegenSpeed")
                speed = value;
            else
                Debug.LogError("Unable to parse health regen value.");
        }

        Component = new ManaRegen(regen, speed);
    }

    public string CreateSerializableData(IComponent component)
    {
        ManaRegen mr = (ManaRegen)component;
        return $"{nameof(ManaRegen)}: {nameof(mr.RegenAmount)}={mr.RegenAmount}, {nameof(mr.RegenSpeed)}={mr.RegenSpeed}";
    }
}