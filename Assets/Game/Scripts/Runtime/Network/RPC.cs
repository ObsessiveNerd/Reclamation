using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RPC : NetworkBehaviour
{
    public GameObject Camera;

    public override void OnNetworkSpawn()
    {
        UnityEngine.Random.InitState(0);

        if(IsOwner)
        { 
            Instantiate(Camera, transform);
        }
    }
}
