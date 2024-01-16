using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    void Awake()
    {
        Services.Register(this);    
    }
    
    public void InvokeCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
