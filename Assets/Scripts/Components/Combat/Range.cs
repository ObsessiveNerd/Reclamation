using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Component
{
    
}

public class DTO_Range : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Range();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Range);
    }
}