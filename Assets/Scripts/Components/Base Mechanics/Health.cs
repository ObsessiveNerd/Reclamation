using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    public int MaxHealth;
    public int CurrentHealth;

    public override int Priority { get { return 10; } }

    private int PercentHealth {get{ return (CurrentHealth / MaxHealth) * 100; } }

    public Health(int maxHealth, int currentHealth = -1)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth > 0 ? currentHealth : maxHealth;

        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.RestoreHealth);
        RegisteredEvents.Add(GameEventId.RegenHealth);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.GetHealth);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach (var damage in (List<Damage>)gameEvent.Paramters[EventParameters.DamageList])
            {
                RecLog.Log($"{Self.Name} took {damage.DamageAmount} damage of type {damage.DamageType}");
                CurrentHealth -= damage.DamageAmount;

                EventBuilder entityTookDamage = new EventBuilder(GameEventId.EntityTookDamage)
                                                    .With(EventParameters.Entity, Self.ID)
                                                    .With(EventParameters.Damage, damage.DamageAmount);
                World.Instance.Self.FireEvent(entityTookDamage.CreateEvent());

                if (CurrentHealth <= 0)
                {
                    EventBuilder swapActivePlayer = new EventBuilder(GameEventId.RotateActiveCharacter)
                                                    .With(EventParameters.UpdateWorldView, true);
                    FireEvent(Self, swapActivePlayer.CreateEvent());

                    FireEvent(Self, new GameEvent(GameEventId.Died, new KeyValuePair<string, object>(EventParameters.DamageSource, gameEvent.GetValue<string>(EventParameters.DamageSource))));
                    Spawner.Despawn(Self);
                    RecLog.Log("...and died");
                    break;
                }
            }
        }

        else if(gameEvent.ID == GameEventId.RegenHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
        }

        else if(gameEvent.ID == GameEventId.RestoreHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);

            EventBuilder entityHealedDamage = new EventBuilder(GameEventId.EntityHealedDamage)
                                                    .With(EventParameters.Entity, Self.ID)
                                                    .With(EventParameters.Healing, healAmount);
            World.Instance.Self.FireEvent(entityHealedDamage.CreateEvent());
        }

        else if(gameEvent.ID == GameEventId.GetHealth)
        {
            gameEvent.Paramters[EventParameters.Value] = CurrentHealth;
            gameEvent.Paramters[EventParameters.MaxValue] = MaxHealth;
        }

        else if(gameEvent.ID == GameEventId.GetCombatRating)
        {
            int startValue = (int)gameEvent.Paramters[EventParameters.Value];
            int modifier = 0;
            if (PercentHealth > 95)
                modifier = 2;
            else if (PercentHealth > 70 && PercentHealth < 95)
                modifier = 1;
            else if (PercentHealth < 50 && !(PercentHealth < 20))
                modifier = -1;
            else if (PercentHealth < 20)
                modifier = -2;

            gameEvent.Paramters[EventParameters.Value] = startValue + modifier;
        }
    }
}


public class DTO_Health : IDataTransferComponent
{
    public IComponent Component { get; set; }
    public void CreateComponent(string data)
    {
        int maxHealth = 0;
        int currentHealth = 0;
        string[] parameters = data.Split(',');
        foreach(string param in parameters)
        {
            string[] value = param.Split('=');
            if(value.Length == 2)
            {
                switch(value[0])
                {
                    case "MaxHealth":
                        maxHealth = int.Parse(value[1]);
                        break;
                    case "CurrentHealth":
                        currentHealth = int.Parse(value[1]);
                        break;
                }
            }
            else
            {
                maxHealth = int.Parse(parameters[0]);
                currentHealth = parameters.Length > 1 ? int.Parse(parameters[1]) : maxHealth;
            }
        }
        Component = new Health(maxHealth, currentHealth);
    }

    public string CreateSerializableData(IComponent component)
    {
        Health health = (Health)component;
        return $"{nameof(Health)}:{health.MaxHealth},{health.CurrentHealth}";
    }
}