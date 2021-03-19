using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    public int MaxHealth;
    public int CurrentHealth;

    public override int Priority { get { return 10; } }

    public Health(int maxHealth, int currentHealth = -1)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth > 0 ? currentHealth : maxHealth;

        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.RestoreHealth);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach (var damage in (List<Damage>)gameEvent.Paramters[EventParameters.DamageList])
            {
                RecLog.Log($"{Self.Name} took {damage.DamageAmount} damage of type {damage.DamageType}");
                CurrentHealth -= damage.DamageAmount;
                if (CurrentHealth <= 0)
                {
                    EventBuilder swapActivePlayer = new EventBuilder(GameEventId.RotateActiveCharacter)
                                                    .With(EventParameters.UpdateWorldView, true);
                    FireEvent(Self, swapActivePlayer.CreateEvent());

                    Spawner.Despawn(Self); //Todo: temp - we need to just send a "I'm dead event" to Self and then have a death handler handle it (that way a goblin dying doesn't trigger a save delete and all that
                    RecLog.Log("...and died");
                    break;
                }
            }
        }

        if(gameEvent.ID == GameEventId.RestoreHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
        }
    }
}


public class DTO_Health : IDataTransferComponent
{
    public IComponent Component { get; set; }
    public void CreateComponent(string data)
    {
        string[] parameters = data.Split(',');
        int maxHealth = int.Parse(parameters[0]);
        int currentHealth = parameters.Length > 1 ? int.Parse(parameters[1]) : maxHealth;
        Component = new Health(maxHealth, currentHealth);
    }

    public string CreateSerializableData(IComponent component)
    {
        Health health = (Health)component;
        return $"{nameof(Health)}:{health.MaxHealth},{health.CurrentHealth}";
    }
}