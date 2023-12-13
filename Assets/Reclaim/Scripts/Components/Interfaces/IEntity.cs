using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface GameObject
//{
//    GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
//    GameEvent FireEvent(GameEvent gameEvent, bool logEvent = false);
//    void HandleEvent(GameEvent gameEvent);
//    void AddComponent(IComponent component);
//    void RemoveComponent(IComponent component);
//    void RemoveComponent(Type component);
//    bool HasComponent(Type component, bool includeComponentsToBeAdded = false);
//    List<IComponent> GetComponents();
//    T GetComponent<T>() where T : IComponent;
//    void CleanupComponents();
//    string Name { get; }
//    string InternalName { get; }
//    string ID { get; }
//    bool NeedsCleanup { get; }
//    string Serialize();
//    void Start();
//    //Action<GameObject> Destroyed { get; set; }
//}

//public class EntityComparer : Comparer<GameObject>
//{
//    public override int Compare(GameObject x, GameObject y)
//    {
//        if (x.InternalName.GetHashCode() < y.InternalName.GetHashCode())
//            return -1;
//        if (x.InternalName == y.InternalName)
//            return 0;

//        return 1;
//    }
//}
