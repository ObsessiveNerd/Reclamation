﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
        RegisteredEvents.Add(GameEventId.ApplyDamage, ApplyDamage);
        RegisteredEvents.Add(GameEventId.StatBoosted, StatBoosted);
        RegisteredEvents.Add(GameEventId.AddMaxHealth, AddMaxHealth);
        RegisteredEvents.Add(GameEventId.RemoveMaxHealth, RemoveMaxHealth);

        //RegisteredEvents.Add(GameEventId.RestoreHealth);
        //RegisteredEvents.Add(GameEventId.RegenHealth);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
        //RegisteredEvents.Add(GameEventId.GetHealth);
        //RegisteredEvents.Add(GameEventId.Rest);
    }

    bool m_IsFlickering = false;
    void ApplyDamage(GameEvent gameEvent)
    {
        if (!m_IsFlickering)
            Services.Coroutine.InvokeCoroutine(FlickerRed());

        CurrentHealth -= gameEvent.GetValue<int>(EventParameter.DamageAmount);
        if(CurrentHealth <= 0)
        {
            var died = GameEventPool.Get(GameEventId.Died);
            gameObject.FireEvent(died);
            died.Release();
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }

    IEnumerator FlickerRed()
    {
        m_IsFlickering = true;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.color = Color.white;
        m_IsFlickering = false;
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