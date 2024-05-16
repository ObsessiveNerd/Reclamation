using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : NetworkBehaviour, IPointerDownHandler
{
    public SO_Item AssignedItem;
    NetworkVariable<bool> m_ItemCreated = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(NetworkManager.IsServer)
            m_ItemCreated.Value = false;
    }

    void Start()
    {
        if (NetworkManager.IsServer && !NetworkObject.IsSpawned)
        {
            NetworkObject.Spawn();

        }
        else if (!NetworkManager.IsServer)
        {
            if (!NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(NetworkObject.NetworkObjectId))
                Destroy(gameObject);
            else if (m_ItemCreated.Value)
            {
                //Item is already created on server side, we need this client to fetch the 
                //data and create the AssignedItem
                UpdateItemServerRpc(NetworkManager.LocalClientId);
            }
        }
        Create(AssignedItem);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateItemServerRpc(ulong clientId)
    {
        var data = AssignedItem.Serialize();
        UpdateItemClientRpc(clientId, AssignedItem.GetType().FullName, data);
    }

    [ClientRpc]
    void UpdateItemClientRpc(ulong clientId, string type, string data)
    {
        if (NetworkManager.LocalClientId == clientId)
        {
            var item = ScriptableObject.CreateInstance(type) as SO_Item;
            item.Deserialize(data);
            Create(item);
        }
    }

    public void Create(SO_Item item)
    {
        if (item != null)
        {
            AssignedItem = item;
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = AssignedItem.Sprite;
            renderer.color = AssignedItem.GetSerializedItem().SpriteColor;
            if (NetworkManager.IsServer)
                m_ItemCreated.Value = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PickupServerRpc()
    {
        NetworkObject.Despawn();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var player = NetworkManager.LocalClient.PlayerObject.gameObject;
        var inventory = player.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.AddToInventory(AssignedItem);
            PickupServerRpc();
        }
    }
}
