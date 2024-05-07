using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Energy))]
//[RequireComponent(typeof(Position))]
public class PlayerInputController : InputControllerBase
{
    Energy m_Energy;
    Camera m_Camera;
    EquipmentBehavior m_Equipment;
    protected override void Start()
    {
        base.Start();
        m_Energy = GetComponent<Energy>();
        m_Equipment = GetComponentInChildren<EquipmentBehavior>();
        m_Camera = FindFirstObjectByType<Camera>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        MoveDirection desiredDirection = InputUtility.GetMoveDirection();
        MoveServerRpc(desiredDirection, inputX, inputY);

        m_Equipment.UpdatePositionServerRpc(m_Camera.ScreenToWorldPoint(Input.mousePosition), transform.position);

        if (GameKeyInputBinder.PerformRequestedAction(RequestedAction.InteractWithCurrentTile))
        {
            Position pos = GetComponent<Position>();
            InteractWithTileServerRpc(pos.component.Point);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var clickRay = FindFirstObjectByType<Camera>().ScreenPointToRay(Input.mousePosition);
                //ScreenToWorldPoint(Input.mousePosition);
            
            //var directionToAttack = (transform.position - worldPosClick).normalized;
            //var postionOfAttack = transform.position + directionToAttack;

            PrimaryActionServerRpc(clickRay.origin, clickRay.direction);

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

    [ClientRpc]
    protected override void PrimaryActionClientRpc(Vector3 origin, Vector3 direction)
    {
        //if(Not holding "force attack" button)
        //{
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, direction);
        if (raycastHit2D.collider != null)
        {
            //Just checking if an object with alternate primary actions is clicked
            var alternatePrimary = GameEventPool.Get(GameEventId.PrimaryInteraction)
                                    .With(EventParameter.ActionList, new List<Action>());
            raycastHit2D.collider.gameObject.FireEvent(alternatePrimary);

            var actionList = alternatePrimary.GetValue<List<Action>>(EventParameter.ActionList);
            if(actionList.Count > 0)
            {
                foreach(var action in actionList)
                {
                    action.Invoke();
                    //gameObject.FireEvent(action);
                    //action.Release();
                }

                return;
            }
        }
        //}

        //if there's no object we actively clicked on,
        //or the object doesn't have alternate primary, we attack it
        GetComponentInChildren<EquipmentBehavior>().Attack();
        
    }
}
