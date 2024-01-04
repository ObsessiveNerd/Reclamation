//using System.Collections.Generic;

//public class FOV : EntityComponent
//{
//    IFovAlgorithm m_Fov;
//    public int FOVRange;

//    public void Start()
//    {
        
//        RegisteredEvents.Add(GameEventId.AfterMoving, UpdateFoV);
//        RegisteredEvents.Add(GameEventId.InitFOV, UpdateFoV);
//        m_Fov = new Shadowcasting();
//    }

//    void UpdateFoV(GameEvent gameEvent)
//    {
//        int baseRange = FOVRange;
//        GameEvent beforeFOVCalculated = GameEventPool.Get(GameEventId.BeforeFOVRecalculated)
//                .With(EventParameter.FOVRange, FOVRange);
//        gameObject.FireEvent(GameEventPool.Get(GameEventId.FOVRecalculated)
//            .With(EventParameter.Entity, gameObject)
//            .With(EventParameter.VisibleTiles, m_Fov.GetVisibleTiles(gameObject, FOVRange)), true).Release();
//        FOVRange = baseRange;

//        beforeFOVCalculated.Release();
//    }
//}