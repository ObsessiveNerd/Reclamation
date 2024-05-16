using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public static Spawner Instance;

    private void Start()
    {
        Instance = this;
    }

    public void Spawn(SO_Item item, Vector2 position)
    {
        SpawnItemServerRpc(item.GetType().FullName, item.Serialize(), position);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemServerRpc(string type, string data, Vector2 position)
    {
        var prefab = Resources.Load<GameObject>("Item");
        var instance = Instantiate(prefab, position, Quaternion.identity);
        var netObj = instance.GetComponent<NetworkObject>();
        netObj.Spawn();
        SpawnItemClientRpc(type, data, netObj.NetworkObjectId);
    }

    [ClientRpc]
    void SpawnItemClientRpc(string type, string data, ulong itemNetID)
    {
        var spawnedObject = NetworkManager.SpawnManager.SpawnedObjects[itemNetID];
        
        var item = ScriptableObject.CreateInstance(type) as SO_Item;
        item.Deserialize(data);

        spawnedObject.GetComponent<SpriteRenderer>().color = item.GetSerializedItem().SpriteColor;
        spawnedObject.GetComponent<Item>().Create(item);
    }
}
