using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InputControllerBase : NetworkBehaviour
{
    protected IMovement m_Move;

    [ServerRpc]
    protected void MoveServerRpc(float inputX, float inputY)
    {
        MoveClientRpc(inputX, inputY);
    }

    [ClientRpc]
    protected void MoveClientRpc(float inputX, float inputY)
    {
        if(m_Move != null)
            m_Move.Move(inputX, inputY);
    }

    [ServerRpc]
    protected void PrimaryActionServerRpc(Ray interactRay)
    {
        PrimaryActionClientRpc(interactRay);
    }

    [ClientRpc]
    protected virtual void PrimaryActionClientRpc(Ray interactRay)
    {
        
    }

    [ServerRpc]
    protected void SecondaryActionServerRpc(Ray interactRay)
    {
        SecondaryActionClientRpc(interactRay);
    }

    [ClientRpc]
    protected virtual void SecondaryActionClientRpc(Ray interactRay)
    {
        
    }
}
