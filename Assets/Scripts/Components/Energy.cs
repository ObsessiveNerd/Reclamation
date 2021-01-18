using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : Component
{
    public float EnergyRegineration;
    public float CurrentEnergy;

    public Energy(float energyRegenValue, float currentEnergy = 0)
    {
        RegisteredEvents.Add(GameEventId.StartTurn);
        RegisteredEvents.Add(GameEventId.HasEnoughEnergyToTakeATurn);
        RegisteredEvents.Add(GameEventId.UseEnergy);
        RegisteredEvents.Add(GameEventId.SkipTurn);
        RegisteredEvents.Add(GameEventId.GetEnergy);
        EnergyRegineration = energyRegenValue;
        CurrentEnergy = currentEnergy;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        GameEvent data;
        switch (gameEvent.ID)
        {
            case GameEventId.StartTurn:
                data = new GameEvent(GameEventId.AlterEnergy, new KeyValuePair<string, object>(EventParameters.EnergyRegen, EnergyRegineration));
                Self.HandleEvent(data);
                CurrentEnergy += (float)data.Paramters[EventParameters.EnergyRegen];
                break;
            case GameEventId.HasEnoughEnergyToTakeATurn:
                data = new GameEvent(GameEventId.GetMinimumEnergyForAction, new KeyValuePair<string, object>(EventParameters.Value, 0f));
                FireEvent(Self, data);
                float minEnergy = (float)data.Paramters[EventParameters.Value];
                gameEvent.Paramters[EventParameters.TakeTurn] = (minEnergy == 0 || CurrentEnergy < minEnergy);
                break;
            case GameEventId.UseEnergy:
                float energyUsed = (float)gameEvent.Paramters[EventParameters.Value];
                CurrentEnergy -= energyUsed;
                break;
            case GameEventId.SkipTurn:
                CurrentEnergy = 0;
                break;
            case GameEventId.GetEnergy:
                gameEvent.Paramters[EventParameters.Value] = CurrentEnergy;
                break;
        };
    }
}

public class DTO_Energy : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] parameters = data.Split(',');
        int energyRegen = 0;
        int currentEnergy = 0;
        foreach(string param in parameters)
        {
            string[] values = param.Split('=');
            switch(values[0])
            {
                case "Regen":
                    energyRegen = int.Parse(values[1]);
                    break;
                case "Current":
                    currentEnergy = int.Parse(values[1]);
                    break;
            }
        }
        Component = new Energy(energyRegen, currentEnergy);
    }

    public string CreateSerializableData(IComponent component)
    {
        Energy e = (Energy)component;
        return $"{nameof(Energy)}:Regen={e.EnergyRegineration}, Current={e.CurrentEnergy}";
    }
}
