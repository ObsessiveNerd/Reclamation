using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : WorldComponent
{
    CameraControllerMono m_CameraControllerMono;
    
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SetCameraPosition);
        //m_CameraControllerMono = GameObject.FindObjectOfType<CameraControllerMono>();
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SetCameraPosition)
        {
            if (m_CameraControllerMono == null)
                m_CameraControllerMono = GameObject.FindObjectOfType<CameraControllerMono>();

            Point p = gameEvent.GetValue<Point>(EventParameters.Point);
            if (!m_GameObjectMap.ContainsKey(p))
                return;
            m_CameraControllerMono.Target = m_GameObjectMap[p];
        }
    }
}
