using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class AnimatedProjectile : MonoBehaviour
{
    public Vector3 Destination;
    public GameObject EntityInstance;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Destination, 20f * Time.deltaTime);
        if (transform.position == Destination)
        {
            var setPosition = GameEventPool.Get(GameEventId.SetEntityPosition)
                .With(EventParameter.Point, new Point(Destination));
            
            EntityInstance.FireEvent(setPosition);
            EntityInstance.Show();
            
            setPosition.Release();

            Destroy(gameObject);
        }
    }
}
