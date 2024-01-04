//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RangedPlayerAttackController : InputControllerBase
//{
//    Point m_TileSelection;
//    GameObject m_Attack;

//    public void Start()
//    {
//        GameObject startingTarget = WorldUtility.GetClosestEnemyTo(gameObject);

//        GameEvent isInFOV = GameEventPool.Get(GameEventId.IsInFOV)
//                                .With(EventParameter.Entity, startingTarget)
//                                .With(EventParameter.Value, false);

//        bool isInFoVResult = FireEvent(gameObject, isInFOV).GetValue<bool>(EventParameter.Value);
//        if (!isInFoVResult)
//            startingTarget = gameObject;
//        isInFOV.Release();

//        m_TileSelection = Services.WorldDataQuery.GetEntityLocation(startingTarget);
//        Services.TileSelectionService.SelectTile(m_TileSelection);
//        Services.WorldUpdateService.UpdateWorldView();
//        UIManager.Push(this);
//    }

//}