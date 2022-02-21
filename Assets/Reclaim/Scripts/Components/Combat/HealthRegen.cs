using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : EntityComponent
{
    public int RegenAmount;
    public int RegenSpeed;

    int currentTurns = 0;
    public HealthRegen(int regenAmount, int speed)
    {
        RegenAmount = regenAmount;
        RegenSpeed = speed;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AfterMoving);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AfterMoving)
        {
            currentTurns++;
            if(currentTurns >= RegenSpeed)
            {
                GameEvent regenHealth = GameEventPool.Get(GameEventId.RegenHealth)
                                            .With(EventParameters.Healing, RegenAmount);
                FireEvent(Self, regenHealth, true).Release();
                currentTurns = 0;
            }
        }
    }
}

public class DTO_HealthRegen : IDataTransferComponent
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

        Component = new HealthRegen(regen, speed);
    }

    public string CreateSerializableData(IComponent component)
    {
        HealthRegen hr = (HealthRegen)component;
        return $"{nameof(HealthRegen)}: {nameof(hr.RegenAmount)}={hr.RegenAmount}, {nameof(hr.RegenSpeed)}={hr.RegenSpeed}";
    }
}
