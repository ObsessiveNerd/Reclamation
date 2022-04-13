using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : EntityComponent
{
    public int MaxHealth;
    public int CurrentHealth;
    int modMultiplier = 5;
    public override int Priority { get { return 10; } }

    private float PercentHealth {get{ return ((float)CurrentHealth / (float)MaxHealth) * 100f; } }

    public Health(int maxHealth, int currentHealth = -1)
    {
        MaxHealth = maxHealth;
        //CurrentHealth = currentHealth > 0 ? currentHealth : maxHealth;
        CurrentHealth = currentHealth;

        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.RestoreHealth);
        RegisteredEvents.Add(GameEventId.RegenHealth);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.GetHealth);
        RegisteredEvents.Add(GameEventId.StatBoosted);
        RegisteredEvents.Add(GameEventId.Rest);
    }

    public override void Start()
    {
        base.Start();
        CurrentHealth = CurrentHealth > 0 ? CurrentHealth : MaxHealth;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach (var damage in (List<Damage>)gameEvent.Paramters[EventParameters.DamageList])
            {
                RecLog.Log($"{Self.Name} took {damage.DamageAmount} damage of type {damage.DamageType}");
                CurrentHealth -= damage.DamageAmount;

                GameEvent playTakeDamageClip = GameEventPool.Get(GameEventId.Playsound)
                                                .With(EventParameters.SoundSource, Self.ID)
                                                .With(EventParameters.Key, SoundKey.AttackHit);
                var weapon = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Attack));
                weapon.FireEvent(playTakeDamageClip);
                playTakeDamageClip.Release();

                Services.WorldUIService.EntityTookDamage(Self, damage.DamageAmount);

                if (CurrentHealth <= 0)
                {
                    GameEvent playDeathSound = GameEventPool.Get(GameEventId.Playsound)
                                                .With(EventParameters.SoundSource, Self.ID)
                                                .With(EventParameters.Key, SoundKey.Died);
                    FireEvent(Self, playDeathSound, true).Release();

                    FireEvent(Self, GameEventPool.Get(GameEventId.Died)
                        .With(EventParameters.DamageSource, gameEvent.GetValue<string>(EventParameters.DamageSource)), true).Release();
                    Spawner.Despawn(Self);
                    RecLog.Log("...and died");
                    break;
                }
            }
        }
        else if (gameEvent.ID == GameEventId.StatBoosted)
        {
            Stats stats = gameEvent.GetValue<Stats>(EventParameters.Stats);
            MaxHealth = 10 + Mathf.Max(0, stats.CalculateModifier(stats.Con) * modMultiplier);
        }

        else if(gameEvent.ID == GameEventId.Rest)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
        }

        else if(gameEvent.ID == GameEventId.RegenHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
            Services.WorldUIService.EntityHealedDamage(Self, healAmount);
        }

        else if (gameEvent.ID == GameEventId.RestoreHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);

            Services.WorldUIService.EntityHealedDamage(Self, healAmount);
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
        return $"{nameof(Health)}:{nameof(health.MaxHealth)}={health.MaxHealth},{nameof(health.CurrentHealth)}={health.CurrentHealth}";
    }
}