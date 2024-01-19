using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class HealthData : EntityComponent
{
    public int MaxHealth;
    public int CurrentHealth;
    public int ModMultiplier = 5;
    public int PercentBoost;

    public Action OnDamaged;
    public Action OnDied;

    public int TotalHealth
    {
        get
        {
            float percent = (float)PercentBoost / 100f;
            int boostAmount = (int)(MaxHealth * percent);
            return MaxHealth + boostAmount;
        }
    }

    Type MonobehaviorType = typeof(Health);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.ApplyDamage, ApplyDamage);
        RegisteredEvents.Add(GameEventId.StatBoosted, StatBoosted);
        RegisteredEvents.Add(GameEventId.AddMaxHealth, AddMaxHealth);
        RegisteredEvents.Add(GameEventId.RemoveMaxHealth, RemoveMaxHealth);
        RegisteredEvents.Add(GameEventId.Died, Died);


        //RegisteredEvents.Add(GameEventId.RestoreHealth);
        //RegisteredEvents.Add(GameEventId.RegenHealth);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
        //RegisteredEvents.Add(GameEventId.GetHealth);
        //RegisteredEvents.Add(GameEventId.Rest);
    }

    void ApplyDamage(GameEvent gameEvent)
    {
        CurrentHealth -= gameEvent.GetValue<int>(EventParameter.DamageAmount);
        OnDamaged();

        if (CurrentHealth <= 0)
        {
            var died = GameEventPool.Get(GameEventId.Died);
            Entity.FireEvent(died);
            died.Release();
            OnDied();
        }
    }
    void StatBoosted(GameEvent gameEvent)
    {
        Stats stats = gameEvent.GetValue<Stats>(EventParameter.Stats);
        MaxHealth = 10 + Mathf.Max(0, stats.CalculateModifier(stats.Con) * ModMultiplier);
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

    void Died(GameEvent gameEvent)
    {
        Tile t = Services.Map.GetTile(Entity.GetComponent<PositionData>().Point);
        t.RemoveObject(Entity.GameObject);
    }
}

public class Health : ComponentBehavior<HealthData>
{
    private void Start()
    {
        component.OnDamaged += Flicker;
        component.OnDied += Died;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        component.OnDamaged -= Flicker;
        component.OnDied -= Died;
    }

    bool m_IsFlickering = false;

    void Flicker()
    {
        if (!m_IsFlickering)
            Services.Coroutine.InvokeCoroutine(FlickerRed());
    }

    void Died()
    {
        NetworkObject.Despawn();
        Destroy(gameObject);
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
}