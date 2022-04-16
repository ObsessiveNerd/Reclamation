using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static HashSet<string> Singletons = new HashSet<string>();

    private void Awake()
    {
        if (Singletons.Contains(gameObject.name))
            Destroy(gameObject);
        else
        {
            var go = transform.parent != null ? transform.parent.gameObject : gameObject;
            Singletons.Add(go.name);
            DontDestroyOnLoad(go);
        }
    }
}
