using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : GameService
{
    CameraControllerMono m_CameraControllerMono;
    
    public void SetCameraPosition(Point p)
    {
        if (m_CameraControllerMono == null)
            m_CameraControllerMono = GameObject.FindObjectOfType<CameraControllerMono>();

        if (!m_GameObjectMap.ContainsKey(p))
            return;
        m_CameraControllerMono.Target = m_GameObjectMap[p];
    }

    public void UpdateCamera()
    {
        if (m_ActivePlayer != null && m_ActivePlayer.Value != null && m_EntityToPointMap.ContainsKey(m_ActivePlayer.Value.ID))
            SetCameraPosition(m_EntityToPointMap[m_ActivePlayer.Value.ID]);
    }
}
