using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVModifier : EntityComponent
{
    public int Modifier;

    public FOVModifier(int mod)
    {
        Modifier = mod;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.BeforeFOVRecalculated);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.BeforeFOVRecalculated)
        {
            int currentMod = (int)gameEvent.Paramters[EventParameters.FOVRange];
            currentMod += Modifier;
            gameEvent.Paramters[EventParameters.FOVRange] = currentMod;
        }
    }
}

public class DTO_FOVModifier : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] keyToValue = data.Split('=');
        int value = int.Parse(keyToValue[1]);
        Component = new FOVModifier(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        FOVModifier mod = (FOVModifier)component;
        return $"{nameof(FOVModifier)}: Modifier={mod.Modifier}";
    }
}
