using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : EntityComponent
{
    public void Start()
    {
        
    }
}

public class DTO_Stackable : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Stackable();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Stackable);
    }
}
