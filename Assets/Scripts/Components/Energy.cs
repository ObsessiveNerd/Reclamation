using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : Component
{
    private float m_EnergyRegineration;
    private float m_CurrentEnergy;

    public Energy(float energyRegenValue)
    {
        RegisteredEvents.Add(GameEventId.StartTurn);
        RegisteredEvents.Add(GameEventId.HasEnoughEnergyToTakeATurn);
        RegisteredEvents.Add(GameEventId.UseEnergy);
        RegisteredEvents.Add(GameEventId.SkipTurn);
        RegisteredEvents.Add(GameEventId.GetEnergy);
        m_EnergyRegineration = energyRegenValue;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        GameEvent data;
        switch (gameEvent.ID)
        {
            case GameEventId.StartTurn:
                data = new GameEvent(GameEventId.AlterEnergy, new KeyValuePair<string, object>(EventParameters.EnergyRegen, m_EnergyRegineration));
                Self.HandleEvent(data);
                m_CurrentEnergy += (float)data.Paramters[EventParameters.EnergyRegen];
                break;
            case GameEventId.HasEnoughEnergyToTakeATurn:
                data = new GameEvent(GameEventId.GetMinimumEnergyForAction, new KeyValuePair<string, object>(EventParameters.Value, 0f));
                FireEvent(Self, data);
                float minEnergy = (float)data.Paramters[EventParameters.Value];
                gameEvent.Paramters[EventParameters.TakeTurn] = (minEnergy == 0 || m_CurrentEnergy < minEnergy);
                break;
            case GameEventId.UseEnergy:
                float energyUsed = (float)gameEvent.Paramters[EventParameters.Value];
                m_CurrentEnergy -= energyUsed;
                break;
            case GameEventId.SkipTurn:
                m_CurrentEnergy = 0;
                break;
            case GameEventId.GetEnergy:
                gameEvent.Paramters[EventParameters.Value] = m_CurrentEnergy;
                break;
        };
    }
}

public class DTO_Energy : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] parameters = data.Split('=');
        Component = new Energy(int.Parse(parameters[1]));
    }
}
