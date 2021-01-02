using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    void FireEvent(IEntity target, GameEvent gameEvent);
    void HandleEvent(GameEvent gameEvent);
    void AddComponent(IComponent component);
    void RemoveComponent(IComponent component);
    void RemoveComponent(Type component);
    void CleanupComponents();
}
