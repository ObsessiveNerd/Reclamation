using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will need to be re-worked some as there should be armor (and weapon) slots instead of adding some of these components directly to the entity
public class Armor : Component
{
    int m_Armor;
    public Armor(int armor)
    {
        m_Armor = armor;
        RegisteredEvents.Add(GameEventId.GetArmor);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.Value] = (int)gameEvent.Paramters[EventParameters.Value] + m_Armor;
    }
}
