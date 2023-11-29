using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : EntityComponent
{
    public string TexturePath;

    Sprite Texture;

    public SpawnEffect(string path)
    {
        TexturePath = path;
        Texture = Resources.Load<Sprite>(path);
    }

    public override void Init(IEntity self)
    {
        RegisteredEvents.Add(GameEventId.FireRangedAttack);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.FireRangedAttack)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/RangedAttack");
            var instance = GameObject.Instantiate(go, gameEvent.GetValue<Vector3>(EventParameter.Target), Quaternion.identity);
            instance.GetComponent<SpriteRenderer>().sprite = Texture;
            instance.AddComponent<SpawnEffectMono>().Setup(gameEvent.GetValue<Vector3>(EventParameter.Target), 2f); 
        }
    }
}

public class DTO_SpawnEffect : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new SpawnEffect(data.Split('=')[1]);
    }

    public string CreateSerializableData(IComponent component)
    {
        SpawnEffect a = (SpawnEffect)component;
        return $"{nameof(SpawnEffect)}: {nameof(a.TexturePath)}={a.TexturePath}";
    }
}
