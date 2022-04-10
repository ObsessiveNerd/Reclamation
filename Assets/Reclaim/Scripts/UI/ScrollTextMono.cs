using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextMono : MonoBehaviour
{
    public float ScrollSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (ScrollSpeed * Time.deltaTime));
    }
}
