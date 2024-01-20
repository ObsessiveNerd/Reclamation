using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;

public struct Test : INetworkSerializable, IEquatable<Test>
{
    public ulong Key;
    public FixedString4096Bytes Value;

    public Test(ulong key, string value)
    {
        Key = key;
        Value = value;
    }

    public bool Equals(Test other)
    {
        if (other.Key == Key)
            return true;
        return false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Key);
        serializer.SerializeValue(ref Value);
    }
}

public class Spawner : NetworkBehaviour
{
    NetworkList<Test> m_NetorkEntityMap; // = new NetworkList<Test>();

    void Awake()
    {
        Services.Register(this);    
        m_NetorkEntityMap = new NetworkList<Test>();
    }

    public GameObject SpawnGameObject(GameObject go, Vector3 position)
    {
        var instance = Instantiate(go, position, Quaternion.identity);
        instance.GetComponent<Position>().component.Point = new Point(position);

        instance.GetComponent<NetworkObject>().Spawn();
        var entity = instance.GetComponent<EntityBehavior>().Entity;
        m_NetorkEntityMap.Add(new Test(instance.GetComponent<NetworkObject>().NetworkObjectId, Services.Serialization.SerializeEntity(entity)));

        //instance.AddComponent<ActivatedNetworkObject>().Activate();
        return instance;
    }

    public void GetEntityFromNetworkId(ulong networkId, out Entity entity)
    {
        foreach(var kvp in m_NetorkEntityMap)
        {
            if(kvp.Key == networkId)
            {
                entity = Services.Serialization.GetEntity(kvp.Value.ToString());
                return;
            }
        }
        entity = null;
    }

    public void RemoveFromNetworkIdMap(ulong networkId)
    {
        Test t = new Test(networkId, "");
        m_NetorkEntityMap.Remove(t);
    }

    public void SpawnGameObject(Entity entity, Point p)
    {
        if (entity.GameObject == null)
            SpawnGameObjectServerRpc(Services.Serialization.SerializeEntity(entity), p);
        else
            SpawnGameObject(entity.GameObject, p.ToVector());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnGameObjectServerRpc(string entityJson, Point p)
    {
        GameObject temp = Resources.Load<GameObject>("SimpleNetworkObject");
        var instance = Instantiate(temp, Services.Map.GetTile(p).transform.position, Quaternion.identity);
        instance.GetComponent<NetworkObject>().Spawn();
        
        ulong networkId = instance.GetComponent<NetworkObject>().NetworkObjectId;
        m_NetorkEntityMap.Add(new Test(networkId, entityJson));
        SpawnGameObjectClientRpc(networkId, entityJson);
        
    }

    [ClientRpc]
    void SpawnGameObjectClientRpc(ulong networkId, string entityJson)
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

        instance.GetComponent<ActivatedNetworkObject>().Activate();
        instance.GetComponent<ActivatedNetworkObject>().ActivateOnSpawn = true;
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
        //RemoveFromNetworkIdMap(networkId);
        //m_NetorkEntityMap.Value.Remove(instance.GetComponent<NetworkObject>().NetworkObjectId);
        instance.GetComponent<NetworkObject>().Despawn();
        if (IsServer)
            Destroy(instance);
        //instance.GetComponent<Position>().component.Point = p;

        //return instance;
    }
}
