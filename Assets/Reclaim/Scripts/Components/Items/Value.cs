using System.Collections;
using System.Collections.Generic;

public class Value : EntityComponent
{
    public int Amount;

    public Value(int amount)
    {
        Amount = amount;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.GetValue);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetValue)
            gameEvent.Paramters[EventParameter.Value] = Amount;
    }
}

public class DTO_Value : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (value.Contains("="))
            value = value.Split('=')[1];
        int amount = int.Parse(value);
        Component = new Value(amount);
    }

    public string CreateSerializableData(IComponent component)
    {
        Value value = (Value)component;
        return $"{nameof(Value)}:{value.Amount}";
    }
}