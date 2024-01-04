using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : EntityComponent
{
    public Sprite Texture;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.FireRangedAttack, FireRangedAttack);
    }

    void FireRangedAttack(GameEvent gameEvent)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/RangedAttack");
        var instance = GameObject.Instantiate(go, gameEvent.GetValue<Vector3>(EventParameter.Target), Quaternion.identity);
        instance.GetComponent<SpriteRenderer>().sprite = Texture;
        instance.AddComponent<SpawnEffectMono>().Setup(gameEvent.GetValue<Vector3>(EventParameter.Target), 2f);
    }
}