using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthData : IComponentData
{
    public int MaxHealth;
    public int CurrentHealth;
    public int ModMultiplier = 5;
    public int PercentBoost;
}

public class Health : EntityComponent
{
    public HealthData Data = new HealthData();

    public int TotalHealth
    {
        get
        {
            float percent = (float)Data.PercentBoost / 100f;
            int boostAmount = (int)(Data.MaxHealth * percent);
            return Data.MaxHealth + boostAmount;
        }
    }

    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.ApplyDamage, ApplyDamage);
        RegisteredEvents.Add(GameEventId.StatBoosted, StatBoosted);
        RegisteredEvents.Add(GameEventId.AddMaxHealth, AddMaxHealth);
        RegisteredEvents.Add(GameEventId.RemoveMaxHealth, RemoveMaxHealth);
        RegisteredEvents.Add(GameEventId.Died, Died);

        if (data != null)
            Data = data as HealthData;

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

        Data.CurrentHealth -= gameEvent.GetValue<int>(EventParameter.DamageAmount);
        if(Data.CurrentHealth <= 0)
        {
            var died = GameEventPool.Get(GameEventId.Died);
            gameObject.FireEvent(died);
            died.Release();
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
    public override IComponentData GetData()
    {
        return Data;
    }

    void Died(GameEvent gameEvent)
    {
        Tile t = Services.Map.GetTile(GetComponent<Position>().Data.Point);
        t.RemoveObject(gameObject);
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
        Data.MaxHealth = 10 + Mathf.Max(0, stats.CalculateModifier(stats.Con) * Data.ModMultiplier);
    }

    void AddMaxHealth(GameEvent gameEvent)
    {
        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
        Data.PercentBoost += amount;
        //Services.WorldUIService.UpdateUI();
    }

    void RemoveMaxHealth(GameEvent gameEvent)
    {
        int amount = gameEvent.GetValue<int>(EventParameter.MaxValue);
        Data.PercentBoost -= amount;
        if (TotalHealth < Data.CurrentHealth)
            Data.CurrentHealth = TotalHealth;
        //Services.WorldUIService.UpdateUI();
    }
}