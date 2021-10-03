using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Component
{
    public string TexturePath;

    Sprite Texture;

    public Ammo(string path)
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
            go.GetComponent<SpriteRenderer>().sprite = Texture;
            go.GetComponent<RangedAttackMono>().Setup(gameEvent.GetValue<Vector3>(EventParameters.Target));
            GameObject.Instantiate(go, gameEvent.GetValue<Vector3>(EventParameters.Entity), Quaternion.identity);
        }
    }
}

public class DTO_Ammo : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Ammo(data.Split('=')[1]);
    }

    public string CreateSerializableData(IComponent component)
    {
        Ammo a = (Ammo)component;
        return $"{nameof(Ammo)}: {nameof(a.TexturePath)}={a.TexturePath}";
    }
}
