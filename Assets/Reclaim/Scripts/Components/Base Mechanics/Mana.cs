using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : EntityComponent
{
    public int MaxMana;
    public int CurrentMana;
    public int PercentBoost;

    int modMultiplier = 5;

    public int TotalMana
    {
        get
        {
            float percent = (float)PercentBoost / 100f;
            int boostAmount = (int)(MaxMana * percent);
            return MaxMana + boostAmount;
        }
    }

    private int PercentMana {get{ return (CurrentMana / MaxMana) * 100; } }

    public override void WakeUp(IComponentData data = null)
    {
        //RegisteredEvents.Add(GameEventId.RestoreMana);
        //RegisteredEvents.Add(GameEventId.DepleteMana);
        //RegisteredEvents.Add(GameEventId.GetMana);
        //RegisteredEvents.Add(GameEventId.StatBoosted);
        //RegisteredEvents.Add(GameEventId.Rest);
        //RegisteredEvents.Add(GameEventId.AddMaxMana);
        //RegisteredEvents.Add(GameEventId.RemoveMaxMana);
    }

    //public override void HandleEvent(GameEvent gameEvent)
    //{
    //    if (gameEvent.ID == GameEventId.RestoreMana)
    //    {
    //        int healAmount = (int)gameEvent.Paramters[EventParameter.Mana];
    //        CurrentMana = Mathf.Min(CurrentMana + healAmount, TotalMana);
    //        Services.WorldUIService.EntityRegainedMana(Self, healAmount);
    //    }

    //    else if (gameEvent.ID == GameEventId.StatBoosted)
    //    {
    //        Stats stats = gameEvent.GetValue<Stats>(EventParameter.Stats);
    //        MaxMana = Mathf.Max(0, stats.CalculateModifier(stats.Int) * modMultiplier);
    //    }
        
    //    else if(gameEvent.ID == GameEventId.AddMaxMana)
    //    {
    //        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
    //        PercentBoost += amount;
    //        Services.WorldUIService.UpdateUI();
    //    }

    //    else if(gameEvent.ID == GameEventId.RemoveMaxMana)
    //    {
    //        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
    //        PercentBoost -= amount;
    //        if(MaxMana < CurrentMana)
    //            CurrentMana = MaxMana;
    //        Services.WorldUIService.UpdateUI();
    //    }

    //    else if(gameEvent.ID == GameEventId.Rest)
    //    {
    //        int healAmount = (int)gameEvent.Paramters[EventParameter.Mana];
    //        CurrentMana = Mathf.Min(CurrentMana + healAmount, TotalMana);
    //    }

    //    else if (gameEvent.ID == GameEventId.DepleteMana)
    //    {
    //        int amountToDrain = gameEvent.GetValue<int>(EventParameter.Mana);
    //        CurrentMana = Mathf.Max(CurrentMana - amountToDrain, 0);
    //    }

    //    else if(gameEvent.ID == GameEventId.GetMana)
    //    {
    //        gameEvent.Paramters[EventParameter.Value] = CurrentMana;
    //        gameEvent.Paramters[EventParameter.MaxValue] = TotalMana;
    //    }
    //}
}
