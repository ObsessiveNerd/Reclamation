using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : EntityComponent
{
    public int RegenAmount;
    public int RegenSpeed;

    int currentTurns = 0;
    public Rest(int regenAmount, int speed)
    {
        RegenAmount = regenAmount;
        RegenSpeed = speed;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
            IEntity target = gameEvent.HasParameter(EventParameters.Entity) ?
                Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity)) : Self;

            currentTurns++;
            if(currentTurns >= RegenSpeed)
            {
                GameEvent rest = GameEventPool.Get(GameEventId.Rest)
                                            .With(EventParameters.Healing, RegenAmount)
                                            .With(EventParameters.Mana, RegenAmount);

                FireEvent(target, rest, true).Release();
                currentTurns = 0;
            }
        }
    }
}

public class DTO_Rest : IDataTransferComponent
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

        Component = new Rest(regen, speed);
    }

    public string CreateSerializableData(IComponent component)
    {
        Rest hr = (Rest)component;
        return $"{nameof(Rest)}: {nameof(hr.RegenAmount)}={hr.RegenAmount}, {nameof(hr.RegenSpeed)}={hr.RegenSpeed}";
    }
}
