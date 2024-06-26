using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    MeleeArea m_MeleeArea;
    Equipment m_Equipment;
    Camera m_Camera;

    InventoryManager m_InventoryManager;
    EquipmentManager m_EqipmentManager;

    InventoryManager InventoryManager
    {
        get
        {
            if(m_InventoryManager == null)
                m_InventoryManager = FindFirstObjectByType<InventoryManager>();
            return m_InventoryManager;
        }
    }

    EquipmentManager EquipmentManager
    {
        get
        {
            if(m_EqipmentManager == null)
                m_EqipmentManager = FindFirstObjectByType<EquipmentManager>();
            return m_EqipmentManager;
        }
    }

    bool m_IsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        m_MeleeArea = GetComponentInChildren<MeleeArea>();
        m_Equipment = GetComponent<Equipment>();
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
        m_MeleeArea.ManualUpdate(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.I))
        {
            if (!m_IsOpen)
            { 
                InventoryManager.Open(gameObject);
                EquipmentManager.Open(gameObject);
            }
            else
            { 
                InventoryManager.Close();
                EquipmentManager.Close();
            }
            m_IsOpen = !m_IsOpen;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            InventoryManager.Close();
            EquipmentManager.Close();
            m_IsOpen = false;
        }

    }

    [ClientRpc]
    protected override void PrimaryActionClientRpc(Ray interactRay)
    {
        m_Equipment.PrimaryAttack();
    }

    [ClientRpc]
    protected override void SecondaryActionClientRpc(Ray interactRay)
    {
        throw new NotImplementedException();
    }
}
