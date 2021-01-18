using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    GameEvent FireEvent(IEntity target, GameEvent gameEvent);
    void HandleEvent(GameEvent gameEvent);
    void AddComponent(IComponent component);
    void RemoveComponent(IComponent component);
    void RemoveComponent(Type component);
    List<IComponent> GetComponents();
    void CleanupComponents();
    string Name { get; }
    string ID { get; }
    bool NeedsCleanup { get; }
    string Serialize();
}
