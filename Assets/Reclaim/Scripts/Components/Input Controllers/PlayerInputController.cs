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

    void Start()
    {
        m_Energy = GetComponent<Energy>();    
    }

    void Update()
    {
        if (!IsOwner)
            return;

        MoveDirection desiredDirection = InputUtility.GetMoveDirection();
        if (desiredDirection != MoveDirection.None && m_Energy.CanTakeATurn)
        {
            MoveServerRpc(desiredDirection);
            m_Energy.TakeTurn();
        }

        if (GameKeyInputBinder.PerformRequestedAction(RequestedAction.InteractWithCurrentTile))
        {
            Position pos = GetComponent<Position>();
            InteractWithTileServerRpc(pos.Point);
        }
    }
}
