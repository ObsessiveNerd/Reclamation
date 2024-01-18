using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity
{
    [SerializeField]
    List<IComponent> Components = new List<IComponent>();
 
    public GameObject GameObject;
    
    public List<IComponent> GetComponents()
    {
        return new List<IComponent>(Components);
    }

    public List<T> GetComponents<T>() where T : Component
    {
        List<T> components = new List<T>();
        foreach (var item in Components)
        {
            if (item.GetType() == typeof(T))
                components.Add(item as T);
        }
        return components;
    }

    public void AddComponent(IComponent component)
    {
        if (component != null)
        {
            component.Entity = this;
            Components.Add(component);
            component.WakeUp();
        }
    }
    public void AddComponentWithoutAwake(IComponent component)
    {
        if (component != null)
        {
            component.Entity = this;
            Components.Add(component);
        }
    }

    public void RemoveComponent(IComponent component)
    {
        Components.Remove(component);
    }

    public T GetComponent<T>() where T : EntityComponent
    {
        foreach (var c in Components)
        {
            if (c.GetType() == typeof(T))
                return c as T;
        }
        return null;
    }

    public GameEvent FireEvent(GameEvent gameEvent, bool logEvent = false)
    {
        foreach(var comp in GetComponents())
                comp.HandleEvent(gameEvent);
        return gameEvent;
    }
}

public class EntityBehavior : MonoBehaviour
{
    public Entity Entity = new Entity();

    // Start is called before the first frame update
    void Awake()
    {
        Entity.GameObject = gameObject;
        ComposeEntityData();
    }

    public Entity ComposeEntityData()
    {
        var componentsToActivate = new List<IComponent>();
        foreach (var component in GetComponents<EntityComponentBehavior>())
        {
            var compData = component.GetData();
            Entity.AddComponentWithoutAwake(compData);
            componentsToActivate.Add(compData);
        }
        foreach (var compData in componentsToActivate)
            compData?.WakeUp();


        return Entity;
    }
}
