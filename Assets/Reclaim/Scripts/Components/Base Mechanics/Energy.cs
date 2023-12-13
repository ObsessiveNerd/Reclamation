using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : EntityComponent
{
    public float EnergyRegineration;
    public float CurrentEnergy;
    public bool HasHadTurnStarted = false;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.StartTurn, StartTurn);
        RegisteredEvents.Add(GameEventId.EndTurn, EndTurn);
        RegisteredEvents.Add(GameEventId.CharacterRotated, CharacterRotated);
        RegisteredEvents.Add(GameEventId.HasEnoughEnergyToTakeATurn, HasEnoughEnergyToTakeATurn);
        RegisteredEvents.Add(GameEventId.UseEnergy, UseEnergy);
        RegisteredEvents.Add(GameEventId.SkipTurn, SkipTurn);
        RegisteredEvents.Add(GameEventId.GetEnergy, GetEnergy);
    }

    void StartTurn(GameEvent gameEvent)
    {
        if (!HasHadTurnStarted)
        {
            GameEvent data = default(GameEvent);
            data = GameEventPool.Get(GameEventId.AlterEnergy).With(EventParameter.EnergyRegen, EnergyRegineration);
            
            gameObject.FireEvent(data);
            CurrentEnergy += (float)data.Paramters[EventParameter.EnergyRegen];
            HasHadTurnStarted = true;

            data?.Release();
        }
    }

    void EndTurn(GameEvent gameEvent)
    {
        HasHadTurnStarted = false;
    }
    void CharacterRotated(GameEvent gameEvent)
    {
        if (!HasHadTurnStarted)
            gameObject.FireEvent(GameEventPool.Get(GameEventId.StartTurn)).Release();
    }
    void HasEnoughEnergyToTakeATurn(GameEvent gameEvent)
    {
        GameEvent data = default(GameEvent);
        data = GameEventPool.Get(GameEventId.GetMinimumEnergyForAction).With(EventParameter.Value, 0f);
        
        gameObject.FireEvent(data);
        float minEnergy = (float)data.Paramters[EventParameter.Value];
        bool takeTurn = (minEnergy == 0 || CurrentEnergy < minEnergy);
        gameEvent.Paramters[EventParameter.TakeTurn] = takeTurn;
        
        data?.Release();
    }
    void UseEnergy(GameEvent gameEvent)
    {
        float energyUsed = (float)gameEvent.Paramters[EventParameter.Value];
        CurrentEnergy -= energyUsed;
    }
    void SkipTurn(GameEvent gameEvent)
    {
        CurrentEnergy = 0;
        HasHadTurnStarted = true;
    }
    void GetEnergy(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Value] = CurrentEnergy;
    }
}
