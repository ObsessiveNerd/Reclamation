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

    public override int Priority { get { return 10; } }

    private int PercentMana {get{ return (CurrentMana / MaxMana) * 100; } }

    public Mana(int maxHealth, int currentHealth = -1, int percentBoost = 0)
    {
        MaxMana = maxHealth;
        //CurrentMana = currentHealth > 0 ? currentHealth : maxHealth;
        CurrentMana = currentHealth;
        PercentBoost = percentBoost;

        RegisteredEvents.Add(GameEventId.RestoreMana);
        RegisteredEvents.Add(GameEventId.DepleteMana);
        RegisteredEvents.Add(GameEventId.GetMana);
        RegisteredEvents.Add(GameEventId.StatBoosted);
        RegisteredEvents.Add(GameEventId.Rest);
        RegisteredEvents.Add(GameEventId.AddMaxMana);
        RegisteredEvents.Add(GameEventId.RemoveMaxMana);
    }

    public override void Start()
    {
        base.Start();
        CurrentMana = CurrentMana > 0 ? CurrentMana : TotalMana;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.RestoreMana)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Mana];
            CurrentMana = Mathf.Min(CurrentMana + healAmount, TotalMana);
            Services.WorldUIService.EntityRegainedMana(Self, healAmount);
        }

        else if (gameEvent.ID == GameEventId.StatBoosted)
        {
            Stats stats = gameEvent.GetValue<Stats>(EventParameters.Stats);
            MaxMana = Mathf.Max(0, stats.CalculateModifier(stats.Int) * modMultiplier);
        }
        
        else if(gameEvent.ID == GameEventId.AddMaxMana)
        {
            int amount = gameEvent.GetValue<int>(EventParameters.MaxValue);
            PercentBoost += amount;
            Services.WorldUIService.UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.RemoveMaxMana)
        {
            int amount = gameEvent.GetValue<int>(EventParameters.MaxValue);
            PercentBoost -= amount;
            if(MaxMana < CurrentMana)
                CurrentMana = MaxMana;
            Services.WorldUIService.UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.Rest)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Mana];
            CurrentMana = Mathf.Min(CurrentMana + healAmount, TotalMana);
        }

        else if (gameEvent.ID == GameEventId.DepleteMana)
        {
            int amountToDrain = gameEvent.GetValue<int>(EventParameters.Mana);
            CurrentMana = Mathf.Max(CurrentMana - amountToDrain, 0);
        }

        else if(gameEvent.ID == GameEventId.GetMana)
        {
            gameEvent.Paramters[EventParameters.Value] = CurrentMana;
            gameEvent.Paramters[EventParameters.MaxValue] = TotalMana;
        }
    }
}


public class DTO_Mana : IDataTransferComponent
{
    public IComponent Component { get; set; }
    public void CreateComponent(string data)
    {
        int maxMana = 0;
        int currentMana = 0;
        int percentBoost = 0;

        string[] parameters = data.Split(',');
        foreach(string param in parameters)
        {
            string[] value = param.Split('=');
            if(value.Length == 2)
            {
                switch(value[0])
                {
                    case "MaxMana":
                        if(!string.IsNullOrEmpty(value[1]))
                            maxMana = int.Parse(value[1]);
                        break;
                    case "CurrentMana":
                        if(!string.IsNullOrEmpty(value[1]))
                            currentMana = int.Parse(value[1]);
                        break;
                    case "PercentBoost":
                        if(!string.IsNullOrEmpty(value[1]))
                            percentBoost = int.Parse(value[1]);
                        break;
                }
            }
            else
            {
                maxMana = int.Parse(parameters[0]);
                currentMana = parameters.Length > 1 ? int.Parse(parameters[1]) : maxMana;
            }
        }
        Component = new Mana(maxMana, currentMana, percentBoost);
    }

    public string CreateSerializableData(IComponent component)
    {
        Mana mana = (Mana)component;
        return $"{nameof(Mana)}:{nameof(mana.MaxMana)}={mana.MaxMana},{nameof(mana.CurrentMana)}={mana.CurrentMana}, {nameof(mana.PercentBoost)}={mana.PercentBoost}";
    }
}
