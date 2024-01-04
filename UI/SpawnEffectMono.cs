using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffectMono : MonoBehaviour
{
    public void Setup(Vector2 destination, float duration)
    {
        transform.position = destination;
        StartCoroutine(DestroyAfter(duration));
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
