using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnEffectMono : MonoBehaviour
{
    public void Setup(GameObject target, float duration)
    {
        transform.position = target.transform.position;
        Services.Coroutine.InvokeCoroutine(DestroyAfter(duration));
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
