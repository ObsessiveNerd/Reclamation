using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    MeleeArea m_Equipment;
    Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Equipment = GetComponentInChildren<MeleeArea>();
        m_Move = GetComponent<IMovement>();
        m_Camera = FindFirstObjectByType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) 
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        MoveServerRpc(x, y);
        if (Input.GetMouseButtonDown(0))
        {
            var ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            PrimaryActionServerRpc(ray);
        }
    }

    [ClientRpc]
    protected override void PrimaryActionClientRpc(Ray interactRay)
    {
        m_Equipment.Attack();
    }

    [ClientRpc]
    protected override void SecondaryActionClientRpc(Ray interactRay)
    {
        throw new NotImplementedException();
    }
}
