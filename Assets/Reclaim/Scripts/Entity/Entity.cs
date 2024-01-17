using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData
{
    [SerializeField]
    public List<IComponentData> Components = new List<IComponentData>();
    
    public GameObject GameObject;
}

public class Entity : MonoBehaviour
{
    public EntityData Data = new EntityData();

    // Start is called before the first frame update
    void Awake()
    {
        Data.GameObject = gameObject;
    }

    public EntityData ComposeEntityData()
    {
        foreach (var component in GetComponents<EntityComponent>())
            Data.Components.Add(component.GetData());

        return Data;
    }

    //public void AddComponentData(ComponentData data)
    //{ 
    //    Data.Components.Add(data);
    //}
}
