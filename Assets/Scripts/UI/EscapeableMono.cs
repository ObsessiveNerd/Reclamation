using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeableMono : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if(gameObject.activeInHierarchy)
                OnEscape();
    }

    protected virtual void OnEscape() { }
}
