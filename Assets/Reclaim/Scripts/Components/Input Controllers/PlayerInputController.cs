﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Energy))]
//[RequireComponent(typeof(Position))]
public class PlayerInputController : InputControllerBase
{
    Energy m_Energy;

    protected override void Start()
    {
        base.Start();
        m_Energy = GetComponent<Energy>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        MoveDirection desiredDirection = InputUtility.GetMoveDirection();
        MoveServerRpc(desiredDirection, inputX, inputY);

        if (GameKeyInputBinder.PerformRequestedAction(RequestedAction.InteractWithCurrentTile))
        {
            Position pos = GetComponent<Position>();
            InteractWithTileServerRpc(pos.component.Point);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var worldPosClick = FindFirstObjectByType<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Debug.Log($"{worldPosClick.x}, {worldPosClick.y}");
            //var directionToAttack = (transform.position - worldPosClick).normalized;
            //var postionOfAttack = transform.position + directionToAttack;

            PrimaryActionServerRpc(worldPosClick);

            //var ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
            //RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            //if(raycastHit2D.collider != null)
            //{
            //    GameObject go = raycastHit2D.collider.gameObject;
            //    if(go != null)
            //    {
            //        Point selectedPoint = new Point((int)go.transform.position.x, (int)go.transform.position.y);
            //        PrimaryActionServerRpc(selectedPoint);
            //    }
            //}
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if(raycastHit2D.collider != null)
            {
                GameObject go = raycastHit2D.collider.gameObject;
                if(go != null)
                {
                    Point selectedPoint = new Point((int)go.transform.position.x, (int)go.transform.position.y);
                    SecondaryActionClientRpc(selectedPoint);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var worldPos = GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition);
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawSphere(worldPos, 0.2f);
        Gizmos.DrawLine(transform.position, worldPos);
    }
#endif
}
