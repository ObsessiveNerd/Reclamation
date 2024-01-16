using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ManagedItem
{
    public string Name;
    public GameObject Object;

    bool m_ObjectActivated = false;

    public GameObject GetItem()
    {
        if(!m_ObjectActivated)
        {
            Object = Services.EntityFactory.Create(Object);
            m_ObjectActivated = true;
        }

        return Object;
    }

    public void FireEvent(GameEvent gameEvent)
    {
        GetItem().FireEvent(gameEvent);
    }
}
