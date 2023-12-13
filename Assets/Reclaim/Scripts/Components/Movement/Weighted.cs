using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weighted : EntityComponent
{
    public float Weight;

    public Weighted(float weight)
    {
        Weight = weight;
    }
}

//public class DTO_Weighted : IDataTransferComponent
//{
//    public IComponent Component { get; set; }

//    public void CreateComponent(string data)
//    {
//        string value = data.Split('=')[1];
//        Component = new Weighted(float.Parse(value));
//    }

//    public string CreateSerializableData(IComponent component)
//    {
//        Weighted w = (Weighted)component;
//        return $"{nameof(Weighted)}:{nameof(w.Weight)}={w.Weight}";
//    }
//}
