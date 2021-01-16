using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    private int m_MaxHealth;
    private int m_CurrentHealth;

    public override int Priority { get { return 10; } }

    public Health(int maxHealth)
    {
        m_MaxHealth = m_CurrentHealth = maxHealth;

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
                m_CurrentHealth -= damage.DamageAmount;
                if (m_CurrentHealth <= 0)
                {
                    GameEvent ge = FireEvent(Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                        new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.None)));
                    FireEvent(World.Instance.Self, ge);
                    RecLog.Log("...and died");
                    break;
                }
            }
        }

        if(gameEvent.ID == GameEventId.RestoreHealth)
        {
            int healAmount = (int)gameEvent.Paramters[EventParameters.Healing];
            m_CurrentHealth = Mathf.Min(m_CurrentHealth + healAmount, m_MaxHealth);
        }
    }
}


public class DTO_Health : IDataTransferComponent
{
    public IComponent Component { get; set; }
    public void CreateComponent(string data)
    {
        int health = int.Parse(data);
        Component = new Health(health);
    }
}