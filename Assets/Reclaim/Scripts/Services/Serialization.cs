using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serialization : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Services.Register(this);
    }

    public string SerializeEntity(Entity entity)
    {
        entity.Serialize();
        return JsonUtility.ToJson(entity);
    }

    public Entity GetEntity(string json)
    {
        Entity entity = JsonUtility.FromJson<Entity>(json);
        entity.DeSerialize();
        return entity;
    }
}
