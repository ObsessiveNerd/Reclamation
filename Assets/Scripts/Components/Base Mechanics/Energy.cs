using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : Component
{
    public float EnergyRegineration;
    public float CurrentEnergy;
    public bool HasHadTurnStarted;

    public Energy(float energyRegenValue, float currentEnergy = 0, bool turnStarted = false)
    {
        RegisteredEvents.Add(GameEventId.StartTurn);
        RegisteredEvents.Add(GameEventId.EndTurn);
        RegisteredEvents.Add(GameEventId.CharacterRotated);
        RegisteredEvents.Add(GameEventId.HasEnoughEnergyToTakeATurn);
        RegisteredEvents.Add(GameEventId.UseEnergy);
        RegisteredEvents.Add(GameEventId.SkipTurn);
        RegisteredEvents.Add(GameEventId.GetEnergy);
        EnergyRegineration = energyRegenValue;
        CurrentEnergy = currentEnergy;
        HasHadTurnStarted = turnStarted;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        GameEvent data;
        switch (gameEvent.ID)
        {
            case GameEventId.StartTurn:
                if (!HasHadTurnStarted)
                {
                    data = new GameEvent(GameEventId.AlterEnergy, new KeyValuePair<string, object>(EventParameters.EnergyRegen, EnergyRegineration));
                    Self.HandleEvent(data);
                    CurrentEnergy += (float)data.Paramters[EventParameters.EnergyRegen];
                    HasHadTurnStarted = true;
                }
                break;
            case GameEventId.HasEnoughEnergyToTakeATurn:
                data = new GameEvent(GameEventId.GetMinimumEnergyForAction, new KeyValuePair<string, object>(EventParameters.Value, 0f));
                FireEvent(Self, data);
                float minEnergy = (float)data.Paramters[EventParameters.Value];
                bool takeTurn = (minEnergy == 0 || CurrentEnergy < minEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = takeTurn;
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
            case GameEventId.EndTurn:
                HasHadTurnStarted = false;
                break;
            case GameEventId.CharacterRotated:
                if(!HasHadTurnStarted)
                    FireEvent(Self, new GameEvent(GameEventId.StartTurn));
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
        float energyRegen = 0;
        float currentEnergy = 0;
        bool turnStarted = false;
        foreach(string param in parameters)
        {
            string[] values = param.Split('=');
            switch(values[0])
            {
                case "EnergyRegineration":
                case "Regen":
                    energyRegen = float.Parse(values[1]);
                    break;
                case "CurrentEnergy":
                case "Current":
                    currentEnergy = float.Parse(values[1]);
                    break;
                case "HasHadTurnStarted":
                case "TurnStarted":
                    turnStarted = bool.Parse(values[1]);
                    break;
            }
        }
        Component = new Energy(energyRegen, currentEnergy, turnStarted);
    }

    public string CreateSerializableData(IComponent component)
    {
        Energy e = (Energy)component;
        return $"{nameof(Energy)}:Regen={e.EnergyRegineration}, Current={e.CurrentEnergy}, TurnStarted={e.HasHadTurnStarted}";
    }
}
