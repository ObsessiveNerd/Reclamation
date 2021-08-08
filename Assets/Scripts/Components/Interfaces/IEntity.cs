using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    GameEvent FireEvent(IEntity target, GameEvent gameEvent, bool logEvent = false);
    GameEvent FireEvent(GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
    void AddComponent(IComponent component);
    void RemoveComponent(IComponent component);
    void RemoveComponent(Type component);
    bool HasComponent(Type component);
    List<IComponent> GetComponents();
    T GetComponent<T>() where T : IComponent;
    void CleanupComponents();
    string Name { get; }
    string ID { get; }
    bool NeedsCleanup { get; }
    string Serialize();
    //Action<IEntity> Destroyed { get; set; }
}
