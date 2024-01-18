using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
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

    public void SpawnGameObject(Point p)
    {
        if(GameObject == null)
        {
            GameObject = new GameObject("Entity {new name}");
            GameObject.AddComponent<SpriteRenderer>();
            GameObject.AddComponent<BoxCollider2D>();

            foreach(var item in Components)
            {
                IComponentBehavior monoBehavior = GameObject.AddComponent(item.MonobehaviorType) as IComponentBehavior;
                monoBehavior.SetComponent(item);
            }
        }

        var instance = GameObject.Instantiate(GameObject, Services.Map.GetTile(p).transform.position, Quaternion.identity);
        GameObject.Destroy(GameObject);
        GameObject = instance;
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
        foreach (var component in GetComponents<IComponentBehavior>())
        {
            Entity.AddComponent(component.GetComponent());
        }

        return Entity;
    }
}
