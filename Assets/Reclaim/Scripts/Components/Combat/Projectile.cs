using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static System.TimeZoneInfo;
using static UnityEngine.GraphicsBuffer;

public enum ProjectileType
{
    Arrow,
    Bolt,
    Rock
}

[Serializable]
public class ProjectileData : EntityComponent
{
    public ProjectileType Type;
}

public class Projectile : EntityComponentBehavior
{
    public ProjectileData Data = new ProjectileData();

    public override IComponent GetData()
    {
        return Data;
    }

    //GameObject m_Target;

    //bool m_ReadyForActivation = false;

    //public override void WakeUpIComponentData data = null()
    //{
    //    m_ReadyForActivation = true;
    //}

    //public void Fire(GameObject source, GameObject target)
    //{
    //    transform.position = source.transform.position;
    //    m_Target = target;
    //}

    //void Update()
    //{
    //    if (m_ReadyForActivation && m_Target != null)
    //    {
    //        Point destinationPoint = new Point((int)m_Target.transform.position.x, (int)m_Target.transform.position.y);

    //        GameEvent setDestination = GameEventPool.Get(GameEventId.MoveEntity)
    //            .With(EventParameter.CanMove, true)
    //            .With(EventParameter.TilePosition, destinationPoint);

    //        gameObject.FireEvent(setDestination);
    //        setDestination.Release();
    //        m_ReadyForActivation = false;
    //    }
    //}
}
