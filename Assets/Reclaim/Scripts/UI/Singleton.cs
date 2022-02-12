using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    static HashSet<string> Singletons = new HashSet<string>();

    private void Awake()
    {
        if (Singletons.Contains(gameObject.name))
            Destroy(gameObject);
        else
        { 
            Singletons.Add(gameObject.name);
            DontDestroyOnLoad(transform.parent != null ? transform.parent.gameObject : gameObject);
        }
    }
}
