using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    private int m_MaxHealth;
    private int m_CurrentHealth;
    EntityType m_Type;

    public override int Priority { get { return 10; } }

    public Health(IEntity self, EntityType type, int maxHealth)
    {
        Init(self);
        m_MaxHealth = m_CurrentHealth = maxHealth;
        m_Type = type;

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
                    FireEvent(World.Instance.Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                        new KeyValuePair<string, object>(EventParameters.EntityType, m_Type)));
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
