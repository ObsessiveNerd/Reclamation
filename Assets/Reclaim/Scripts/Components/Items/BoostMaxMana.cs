using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostMaxMana : EntityComponent
{
    public int PercentToBoost;
    public int CalculatedAmount;

    public BoostMaxMana(int percent, int calculated)
    {
        PercentToBoost = percent;
        CalculatedAmount = calculated;
    }

    public override void Init(GameObject self)
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
            GameObject owner = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Owner));
            GameEvent getHealth = GameEventPool.Get(GameEventId.GetMana)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);

            owner.FireEvent(getHealth);
            //float percent = ((float)PercentToBoost / 100f);
            //CalculatedAmount = (int)(getHealth.GetValue<int>(EventParameters.MaxValue) * percent);

            GameEvent boostHealth = GameEventPool.Get(GameEventId.AddMaxMana)
                                    .With(EventParameter.MaxValue, PercentToBoost);
            owner.FireEvent(boostHealth);

            getHealth.Release();
            boostHealth.Release();
        }
        else if(gameEvent.ID == GameEventId.DeactivateObject)
        {
            GameObject owner = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Owner));
            GameEvent removeBoost = GameEventPool.Get(GameEventId.RemoveMaxMana)
                                    .With(EventParameter.MaxValue, PercentToBoost);
            owner.FireEvent(removeBoost);
            removeBoost.Release();
        }
        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(BoostMaxMana)}{Guid.NewGuid()}", $"Boost maximum mana by {PercentToBoost}%");
        }
    }
}

public class DTO_BoostMaxMana : IDataTransferComponent
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
        Component = new BoostMaxMana(percent, calculatedAmount);
    }

    public string CreateSerializableData(IComponent component)
    {
        BoostMaxMana bmh = (BoostMaxMana)component;
        return $"{nameof(BoostMaxMana)}: {nameof(bmh.PercentToBoost)}={bmh.PercentToBoost}";
    }
}
