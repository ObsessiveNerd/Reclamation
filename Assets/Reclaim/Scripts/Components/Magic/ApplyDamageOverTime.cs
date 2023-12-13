using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamageOverTime : EntityComponent
{
    public string Amount;
    public int RemoveAfterTurns;

    public ApplyDamageOverTime(string amount, int removeAfterTurns)
    {
        Amount = amount;
        RemoveAfterTurns = removeAfterTurns;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.CastSpellEffect)
        {
            GameObject target = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Target));
            target.AddComponent(new DamageOverTime(Amount, 0, RemoveAfterTurns));
        }
    }
}

public class DTO_ApplyDamageOverTime : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string amount = "";
        int removeAfterTurns = 0;

        string[] kvp = data.Split(',');
        foreach(var kvpair in kvp)
        {
            string key = kvpair.Split('=')[0];
            string value = kvpair.Split('=')[1];

            switch(key)
            {
                case "Amount":
                    amount = value;
                    break;
                case "RemoveAfterTurns":
                    removeAfterTurns = int.Parse(value);
                    break;
            }
        }
        Component = new ApplyDamageOverTime(amount, removeAfterTurns);
    }

    public string CreateSerializableData(IComponent component)
    {
        ApplyDamageOverTime sb = (ApplyDamageOverTime)component;
        return $"{nameof(ApplyDamageOverTime)}: {nameof(sb.Amount)}={sb.Amount}, {nameof(sb.RemoveAfterTurns)}={sb.RemoveAfterTurns}";
    }
}
