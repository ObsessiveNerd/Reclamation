using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : NetworkBehaviour
{
    public GameObject SimpleNetworkPrefab;

    void Awake()
    {
        Services.Register(this);
    }

    public void SpawnEffect(GameObject effect, Vector3 pos, Quaternion rot)
    {
        var instance = Instantiate(effect, pos, rot);
        instance.AddComponent<DestroyAfter>().Begin(1f);
    }

    public GameObject SpawnGameObject(GameObject go, Vector3 position)
    {
        var instance = Instantiate(go, position, Quaternion.identity);
        instance.GetComponent<Position>().component.Point = new Point(position);
        instance.GetComponent<NetworkObject>().Spawn();
        return instance;
    }

    public void SpawnGameObject(Entity entity, Point p)
    {
        if (IsServer)
            SpawnGameObject(Services.Serialization.SerializeEntity(entity), p);
        else
            SpawnGameObjectServerRpc(Services.Serialization.SerializeEntity(entity), p);
    }

    public void SpawnGameObject(string entityJson, Point p)
    {
        var instance = Instantiate(SimpleNetworkPrefab, Services.Map.GetTile(p).transform.position, Quaternion.identity);
        instance.GetComponent<EntityBehavior>().EntityJson.Value = entityJson;
        instance.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnGameObjectServerRpc(string entityJson, Point p)
    {
        SpawnGameObject(entityJson, p);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyGameObjectServerRpc(ulong networkId)
    {
        NetworkObject netObject;
        GameObject instance;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out netObject))
        {
            Debug.LogError($"No gameobject for networkId {networkId} found!");
            return;
        }
        else
            instance = netObject.gameObject;

        instance.GetComponent<NetworkObject>().Despawn();
        if (IsServer)
            Destroy(instance);
    }
}
