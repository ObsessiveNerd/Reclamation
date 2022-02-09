using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMono : MonoBehaviour
{
    public GameObject Target
    {
        set
        {
            m_Position.x = value.transform.position.x;
            m_Position.y = value.transform.position.y;

            transform.position = m_Position;
        }
    }
    private Vector3 m_Position;

    private void Start()
    {
        m_Position = transform.position;
    }
}
