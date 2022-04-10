using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : EntityComponent
{
    public int MaxMana;
    public int CurrentMana;

    int modMultiplier = 5;

    public override int Priority { get { return 10; } }

    private int PercentMana {get{ return (CurrentMana / MaxMana) * 100; } }

    public Mana(int maxHealth, int currentHealth = -1)
    {
        MaxMana = maxHealth;
        //CurrentMana = currentHealth > 0 ? currentHealth : maxHealth;
        CurrentMana = currentHealth;

        RegisteredEvents.Add(GameEventId.RestoreMana);
        RegisteredEvents.Add(GameEventId.DepleteMana);
        RegisteredEvents.Add(GameEventId.GetMana);
        RegisteredEvents.Add(GameEventId.StatBoosted);
    }

    public override void Start()
    {
        base.Start();
        CurrentMana = CurrentMana > 0 ? CurrentMana : MaxMana;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.RestoreMana)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Mana];
            CurrentMana = Mathf.Min(CurrentMana + healAmount, MaxMana);
        }

        else if (gameEvent.ID == GameEventId.StatBoosted)
        {
            Stats stats = gameEvent.GetValue<Stats>(EventParameters.Stats);
            MaxMana = Mathf.Max(0, stats.CalculateModifier(stats.Int) * modMultiplier);
        }

        else if (gameEvent.ID == GameEventId.DepleteMana)
        {
            int amountToDrain = gameEvent.GetValue<int>(EventParameters.Mana);
            CurrentMana = Mathf.Max(CurrentMana - amountToDrain, 0);
        }

        else if(gameEvent.ID == GameEventId.GetMana)
        {
            gameEvent.Paramters[EventParameters.Value] = CurrentMana;
            gameEvent.Paramters[EventParameters.MaxValue] = MaxMana;
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
        string[] parameters = data.Split(',');
        foreach(string param in parameters)
        {
            string[] value = param.Split('=');
            if(value.Length == 2)
            {
                switch(value[0])
                {
                    case "MaxMana":
                        maxMana = int.Parse(value[1]);
                        break;
                    case "CurrentMana":
                        currentMana = int.Parse(value[1]);
                        break;
                }
            }
            else
            {
                maxMana = int.Parse(parameters[0]);
                currentMana = parameters.Length > 1 ? int.Parse(parameters[1]) : maxMana;
            }
        }
        Component = new Mana(maxMana, currentMana);
    }

    public string CreateSerializableData(IComponent component)
    {
        Mana mana = (Mana)component;
        return $"{nameof(Mana)}:{nameof(mana.MaxMana)}={mana.MaxMana},{nameof(mana.CurrentMana)}={mana.CurrentMana}";
    }
}
