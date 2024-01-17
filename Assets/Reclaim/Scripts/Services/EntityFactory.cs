using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Services.Register(this);
    }

    public GameObject Create(GameObject gameObject, Vector3 position, GameObject parent = null)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<NetworkObject>().Spawn();
        go.transform.position = position;
        
        if(parent != null)
            go.transform.SetParent(parent.transform, false);
        
        go.WakeUp();
        go.Hide();
        return go;
    }
}
