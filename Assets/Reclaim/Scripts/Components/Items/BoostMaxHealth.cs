using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostMaxHealth : EntityComponent
{
    public int PercentToBoost;
    public int CalculatedAmount;

    public BoostMaxHealth(int percent, int calculated)
    {
        PercentToBoost = percent;
        CalculatedAmount = calculated;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ActivateObject);
        RegisteredEvents.Add(GameEventId.DeactivateObject);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ActivateObject)
        {
            IEntity owner = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Owner));
            GameEvent getHealth = GameEventPool.Get(GameEventId.GetHealth)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);

            owner.FireEvent(getHealth);
            //float percent = ((float)PercentToBoost / 100f);
            //CalculatedAmount = (int)(getHealth.GetValue<int>(EventParameters.MaxValue) * percent);

            GameEvent boostHealth = GameEventPool.Get(GameEventId.AddMaxHealth)
                                    .With(EventParameter.MaxValue, PercentToBoost);
            owner.FireEvent(boostHealth);

            getHealth.Release();
            boostHealth.Release();
        }
        else if(gameEvent.ID == GameEventId.DeactivateObject)
        {
            IEntity owner = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Owner));
            GameEvent removeBoost = GameEventPool.Get(GameEventId.RemoveMaxHealth)
                                    .With(EventParameter.MaxValue, PercentToBoost);
            owner.FireEvent(removeBoost);
            removeBoost.Release();
        }
        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(BoostMaxHealth)}{Guid.NewGuid()}", $"Boost maximum health by {PercentToBoost}%");
        }
    }
}

public class DTO_BoostMaxHealth : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int percent = 0;
        int calculatedAmount = 0;

        string[] kvp = data.Split(',');
        foreach (var kvpair in kvp)
        {
            string key = kvpair.Split('=')[0];
            string value = kvpair.Split('=')[1];

            switch(key)
            {
                case "PercentToBoost":
                    percent = int.Parse(value);
                    break;

                case "CalculatedAmount":
                    if(!string.IsNullOrEmpty(value))
                        calculatedAmount = int.Parse(value);
                    break;
            }
        }
        Component = new BoostMaxHealth(percent, calculatedAmount);
    }

    public string CreateSerializableData(IComponent component)
    {
        BoostMaxHealth bmh = (BoostMaxHealth)component;
        return $"{nameof(BoostMaxHealth)}: {nameof(bmh.PercentToBoost)}={bmh.PercentToBoost}";
    }
}
