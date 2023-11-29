public class BlocksVisibility : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.BlocksVision);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BlocksVision)
            gameEvent.Paramters[EventParameter.Value] = true;
    }
}

public class DTO_BlocksVisibility : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new BlocksVisibility();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(BlocksVisibility);
    }
}