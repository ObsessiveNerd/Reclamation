using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Entity //: ISerializationCallbackReceiver
{
    [NonSerialized]
    public bool IsActive = false;

    PositionData m_PositionData;
    public Point position
    {
        get
        {
            if(m_PositionData == null) 
                m_PositionData = GetComponent<PositionData>();
            return m_PositionData.Point;
        }
    }

    //NameData m_NameData;
    //public string name
    //{
    //    get
    //    {
    //        if(m_NameData == null) 
    //            m_NameData = GetComponent<NameData>();
    //        return m_NameData.Name;
    //    }
    //}

    List<EntityComponent> Components = new List<EntityComponent>();
 
    public GameObject GameObject;

    [SerializeField]
    private List<string> componentTypes = new List<string>();
    [SerializeField]
    private List<string> componentData = new List<string>();
    
    public List<EntityComponent> GetComponents()
    {
        return new List<EntityComponent>(Components);
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

    public void AddComponent(EntityComponent component)
    {
        if (component != null)
        {
            component.Entity = this;
            Components.Add(component);
            component.WakeUp();
        }
    }
    public void AddComponentWithoutAwake(EntityComponent component)
    {
        if (component != null)
        {
            component.Entity = this;
            Components.Add(component);
        }
    }

    public void RemoveComponent(EntityComponent component)
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

    public void DeSerialize()
    {
        for(int i = 0; i < componentTypes.Count; i++)
        {
            Type type = Type.GetType(componentTypes[i]);
            EntityComponent component = (EntityComponent)JsonUtility.FromJson(componentData[i], type);
            component.DeSerialzie();
            AddComponent(component);
        }
        componentData.Clear();
        componentTypes.Clear();
    }

    public void Serialize()
    {
        GameObject = null;
        foreach (var component in Components)
        {
            component.Serialzie();
            componentTypes.Add(component.GetType().ToString());
            componentData.Add(JsonUtility.ToJson(component));
        }
        Components.Clear();
    }
}

public class EntityBehavior : MonoBehaviour
{
    public Entity Entity;

    //// Start is called before the first frame update
    void Start()
    {
        if (Entity.IsActive)
            return;

        Entity.GameObject = gameObject;
        ComposeEntityData();
    }

    public void Activate(Entity entity = null)
    {
        if (entity != null)
            Entity = entity;
        Entity.GameObject = gameObject;
        if (!Entity.IsActive)
            ComposeEntityData();
    }

    public Entity ComposeEntityData()
    {
        foreach (var component in GetComponents<IComponentBehavior>())
            Entity.AddComponent(component.GetComponent());
        Entity.IsActive = true;
        return Entity;
    }
}
