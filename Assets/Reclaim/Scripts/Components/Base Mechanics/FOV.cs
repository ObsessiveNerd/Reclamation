using System.Collections.Generic;

public class FOV : EntityComponent
{
    IFovAlgorithm m_Fov;
    public int FOVRange;

    public FOV(int visibleRange)
    {
        FOVRange = visibleRange;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AfterMoving);
        RegisteredEvents.Add(GameEventId.InitFOV);
        m_Fov = new Shadowcasting();

        //FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.FOVRecalculated, new .With(EventParameters.Entity, Self),
        //                                                                                new .With(EventParameters.VisibleTiles, m_Fov.GetVisibleTiles(Self, FOVRange))));
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AfterMoving || gameEvent.ID == GameEventId.InitFOV)
        {
            int baseRange = FOVRange;
            GameEvent beforeFOVCalculated = GameEventPool.Get(GameEventId.BeforeFOVRecalculated)
                .With(EventParameters.FOVRange, FOVRange);
            //GameEvent checkEquipment = (GameEvent)FireEvent(Self, GameEventPool.Get(GameEventId.CheckEquipment)
            //    .With(EventParameters.GameEvent, beforeFOVCalculated)).Paramters[EventParameters.GameEvent];
            //FOVRange = (int)checkEquipment.Paramters[EventParameters.FOVRange];
            FireEvent(Self, GameEventPool.Get(GameEventId.FOVRecalculated)
                .With(EventParameters.Entity, Self.ID)
                .With(EventParameters.VisibleTiles, m_Fov.GetVisibleTiles(Self, FOVRange))).Release();
            FOVRange = baseRange;

            beforeFOVCalculated.Release();
            //checkEquipment.Release();
        }
    }
}

public class DTO_FOV : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int visibleRange = int.Parse(data.Split('=')[1]);
        Component = new FOV(visibleRange);
    }

    public string CreateSerializableData(IComponent component)
    {
        FOV fov = (FOV)component;
        return $"{nameof(FOV)}: FOVRange={fov.FOVRange}";
    }
}