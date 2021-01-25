public class BlocksVisibility : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.CheckVisibility);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.CheckVisibility)
            gameEvent.Paramters[EventParameters.Value] = false;
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