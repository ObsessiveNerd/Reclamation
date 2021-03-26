using System.Collections;
using System.Collections.Generic;

public class Value : Component
{
    public int Amount;

    public override void Init(IEntity self)
    {
        base.Init(self);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {

    }
}

public class DTO_Value : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int amount = int.Parse(data);
        Component = new Gold(amount);
    }

    public string CreateSerializableData(IComponent component)
    {
        Gold gold = (Gold)component;
        return $"{nameof(Gold)}:{gold.Amount}";
    }
}