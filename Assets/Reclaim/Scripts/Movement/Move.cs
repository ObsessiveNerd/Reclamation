using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour, IMovement
{
    public float Speed;
    Rigidbody2D m_RigidBody2d;
    void Start()
    {
        m_RigidBody2d = GetComponent<Rigidbody2D>();
    }

    void IMovement.Move(float x, float y)
    {
        if(m_RigidBody2d != null)
           m_RigidBody2d.velocity = new Vector2 (x, y);
    }
}
