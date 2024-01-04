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

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.TakeDamage, TakeDamage);
        RegisteredEvents.Add(GameEventId.StatBoosted, StatBoosted);
        RegisteredEvents.Add(GameEventId.AddMaxHealth, AddMaxHealth);
        RegisteredEvents.Add(GameEventId.RemoveMaxHealth, RemoveMaxHealth);

        //RegisteredEvents.Add(GameEventId.RestoreHealth);
        //RegisteredEvents.Add(GameEventId.RegenHealth);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
        //RegisteredEvents.Add(GameEventId.GetHealth);
        //RegisteredEvents.Add(GameEventId.Rest);
    }

    void TakeDamage(GameEvent gameEvent)
    {
        foreach (var damage in (List<Damage>)gameEvent.Paramters[EventParameter.DamageList])
        {
            RecLog.Log($"{gameObject.name} took {damage.DamageAmount} damage of type {damage.DamageType}");
            CurrentHealth -= damage.DamageAmount;

            GameObject weapon = gameEvent.GetValue<GameObject>(EventParameter.Attack);
            GameEvent dealtDamage = GameEventPool.Get(GameEventId.DealtDamage)
                                        .With(EventParameter.DamageSource, gameEvent.GetValue<GameObject>(EventParameter.DamageSource))
                                        .With(EventParameter.Damage, damage);
            weapon.FireEvent(dealtDamage).Release();

            //GameEvent playTakeDamageClip = GameEventPool.Get(GameEventId.Playsound)
            //                                    .With(EventParameter.SoundSource, Self.ID)
            //                                    .With(EventParameter.Key, SoundKey.AttackHit);
            //weapon.FireEvent(playTakeDamageClip);

            //playTakeDamageClip.Release();

            //Services.WorldUIService.EntityTookDamage(gameObject, damage.DamageAmount);

            //if (CurrentHealth <= 0)
            //{
            //    GameEvent playDeathSound = GameEventPool.Get(GameEventId.Playsound)
            //                                    .With(EventParameter.SoundSource, gameObject)
            //                                    .With(EventParameter.Key, SoundKey.Died);
            //    gameObject.FireEvent(playDeathSound, true).Release();

            //    gameObject.FireEvent(GameEventPool.Get(GameEventId.Died)
            //        .With(EventParameter.DamageSource, gameEvent.GetValue<string>(EventParameter.DamageSource)), true).Release();
            //    Services.SpawnerService.RegisterForDespawn(gameObject);
            //    break;
            //}
        }
    }

    void StatBoosted(GameEvent gameEvent)
    {
        Stats stats = gameEvent.GetValue<Stats>(EventParameter.Stats);
        MaxHealth = 10 + Mathf.Max(0, stats.CalculateModifier(stats.Con) * modMultiplier);
    }

    void AddMaxHealth(GameEvent gameEvent)
    {
        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
        PercentBoost += amount;
        //Services.WorldUIService.UpdateUI();
    }

    void RemoveMaxHealth(GameEvent gameEvent)
    {
        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
        PercentBoost -= amount;
        if (TotalHealth < CurrentHealth)
            CurrentHealth = TotalHealth;
        //Services.WorldUIService.UpdateUI();
    }
}