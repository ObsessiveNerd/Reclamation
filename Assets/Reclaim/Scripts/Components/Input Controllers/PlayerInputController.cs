using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Energy))]
[RequireComponent(typeof(Position))]
public class PlayerInputController : InputControllerBase
{
    Energy m_Energy;

    public void Start()
    {
        m_Energy = GetComponent<Energy>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        MoveDirection desiredDirection = InputUtility.GetMoveDirection();
        if (desiredDirection != MoveDirection.None && m_Energy.component.CanTakeATurn)
        {
            MoveServerRpc(desiredDirection);
            m_Energy.component.TakeTurn();
        }

        if (GameKeyInputBinder.PerformRequestedAction(RequestedAction.InteractWithCurrentTile))
        {
            Position pos = GetComponent<Position>();
            InteractWithTileServerRpc(pos.component.Point);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if(raycastHit2D.collider != null)
            {
                GameObject go = raycastHit2D.collider.gameObject;
                if(go != null)
                {
                    Point selectedPoint = new Point((int)go.transform.position.x, (int)go.transform.position.y);
                    Tile t = Services.Map.GetTile(selectedPoint);
                    var primaryAction = GameEventPool.Get(GameEventId.PrimaryInteraction)
                        .With(EventParameter.Source, gameObject);
                    t.FireEvent(gameObject, primaryAction);
                    primaryAction.Release();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var e = GetComponent<Inventory>().component.InventoryEntities[0];
            var drop = GameEventPool.Get(GameEventId.Drop)
                .With(EventParameter.Source, GetComponent<EntityBehavior>().Entity);
            e.FireEvent(drop);
            drop.Release();
        }
    }
}
