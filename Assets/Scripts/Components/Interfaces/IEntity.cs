﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    GameEvent FireEvent(IEntity target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
    void AddComponent(IComponent component);
    void RemoveComponent(IComponent component);
    void RemoveComponent(Type component);
    bool HasComponent(Type component);
    List<IComponent> GetComponents();
    void CleanupComponents();
    string Name { get; }
    string ID { get; }
    bool NeedsCleanup { get; }
    string Serialize();
    //Action<IEntity> Destroyed { get; set; }
}
