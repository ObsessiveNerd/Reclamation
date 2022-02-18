using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkId : EntityComponent
{
    public string ID;

    public NetworkId(string id)
    {
        ID = id;
    }
}

public class DTO_NetworkId : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new NetworkId(data.Split('=')[1]);
    }

    public string CreateSerializableData(IComponent component)
    {
        NetworkId id = (NetworkId)component;
        return $"{nameof(NetworkId)}: {nameof(id.ID)}={id.ID}";
    }
}
