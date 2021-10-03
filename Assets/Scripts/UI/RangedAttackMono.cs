using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackMono : MonoBehaviour
{
    Vector2 startPos;
    float distance;
    float speed = 1.0f;

    public void Setup(Vector2 destination)
    {
        startPos = transform.position;
        distance = Vector2.Distance(startPos, destination);
        GetComponent<Rigidbody2D>().velocity = (destination - (Vector2)transform.position)* speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(startPos, transform.position) >= distance)
            Destroy(gameObject);
    }
}
