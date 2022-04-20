using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : EntityComponent
{
    public int MaxHealth;
    public int CurrentHealth;
    int modMultiplier = 5;

    public int PercentBoost;

    public int TotalHealth
    {
        get
        {
            float percent = (float)PercentBoost / 100f;
            int boostAmount = (int)(MaxHealth * percent);
            return MaxHealth + boostAmount;
        }
    }

    public override int Priority { get { return 10; } }

    private float PercentHealth {get{ return ((float)CurrentHealth / (float)TotalHealth) * 100f; } }

    public Health(int maxHealth, int currentHealth = -1, int percentBoost = 0)
    {
        MaxHealth = maxHealth;
        //CurrentHealth = currentHealth > 0 ? currentHealth : maxHealth;
        CurrentHealth = currentHealth;
        PercentBoost = percentBoost;

        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.RestoreHealth);
        RegisteredEvents.Add(GameEventId.RegenHealth);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.GetHealth);
        RegisteredEvents.Add(GameEventId.StatBoosted);
        RegisteredEvents.Add(GameEventId.Rest);
        RegisteredEvents.Add(GameEventId.AddMaxHealth);
        RegisteredEvents.Add(GameEventId.RemoveMaxHealth);
    }

    public override void Start()
    {
        base.Start();
        CurrentHealth = CurrentHealth > 0 ? CurrentHealth : TotalHealth;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach (var damage in (List<Damage>)gameEvent.Paramters[EventParameters.DamageList])
            {
                RecLog.Log($"{Self.Name} took {damage.DamageAmount} damage of type {damage.DamageType}");
                CurrentHealth -= damage.DamageAmount;

                IEntity weapon = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Attack));
                GameEvent dealtDamage = GameEventPool.Get(GameEventId.DealtDamage)
                                        .With(EventParameters.DamageSource, gameEvent.GetValue<string>(EventParameters.DamageSource))
                                        .With(EventParameters.Damage, damage);
                weapon.FireEvent(dealtDamage).Release();

                GameEvent playTakeDamageClip = GameEventPool.Get(GameEventId.Playsound)
                                                .With(EventParameters.SoundSource, Self.ID)
                                                .With(EventParameters.Key, SoundKey.AttackHit);
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
                    Services.SpawnerService.RegisterForDespawn(Self);
                    break;
                }
            }
        }
        else if (gameEvent.ID == GameEventId.StatBoosted)
        {
            Stats stats = gameEvent.GetValue<Stats>(EventParameters.Stats);
            MaxHealth = 10 + Mathf.Max(0, stats.CalculateModifier(stats.Con) * modMultiplier);
        }

        else if(gameEvent.ID == GameEventId.AddMaxHealth)
        {
            int amount = gameEvent.GetValue<int>(EventParameters.MaxValue);
            PercentBoost += amount;
            Services.WorldUIService.UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.RemoveMaxHealth)
        {
            int amount = gameEvent.GetValue<int>(EventParameters.MaxValue);
            PercentBoost -= amount;
            if(TotalHealth < CurrentHealth)
                CurrentHealth = TotalHealth;
            Services.WorldUIService.UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.Rest)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, TotalHealth);
        }

        else if(gameEvent.ID == GameEventId.RegenHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, TotalHealth);
            Services.WorldUIService.EntityHealedDamage(Self, healAmount);
        }

        else if (gameEvent.ID == GameEventId.RestoreHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            CurrentHealth = Mathf.Min(CurrentHealth + healAmount, TotalHealth);

            Services.WorldUIService.EntityHealedDamage(Self, healAmount);
        }

        else if(gameEvent.ID == GameEventId.GetHealth)
        {
            gameEvent.Paramters[EventParameters.Value] = CurrentHealth;
            gameEvent.Paramters[EventParameters.MaxValue] = TotalHealth;
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
        int percentBoost = 0;

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
                    case "PercentBoost":
                        if(!string.IsNullOrEmpty(value[1]))
                            percentBoost = int.Parse(value[1]);
                        break;
                }
            }
            else
            {
                maxHealth = int.Parse(parameters[0]);
                currentHealth = parameters.Length > 1 ? int.Parse(parameters[1]) : maxHealth;
            }
        }
        Component = new Health(maxHealth, currentHealth, percentBoost);
    }

    public string CreateSerializableData(IComponent component)
    {
        Health health = (Health)component;
        return $"{nameof(Health)}:{nameof(health.MaxHealth)}={health.MaxHealth},{nameof(health.CurrentHealth)}={health.CurrentHealth}, {nameof(health.PercentBoost)}={health.PercentBoost}";
    }
}