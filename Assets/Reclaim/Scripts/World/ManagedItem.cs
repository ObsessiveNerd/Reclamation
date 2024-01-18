using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableData
{
    public Type ComponentType;
    public string Data;
}


[Serializable]
public class SerializableGameObject
{
    [SerializeField]
    public List<SerializableData> Data;

    //public SerializableGameObject(GameObject go)
    //{
    //    Data = new List<SerializableData>();

    //    foreach (var component in go.GetComponents<ComponentBehavior>())
    //    {
    //        SerializableData data = new SerializableData();
    //        component.ComponentType = component.GetType();
    //        component.Data = JsonUtility.ToJson(component);
    //        component.Add(data);
    //    }
    //}
}
