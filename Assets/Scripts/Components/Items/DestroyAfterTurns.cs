using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTurns : Component
{
    public int CurrentTurnCount;
    public int DestroyAfterTurnCount;

    public override int Priority => 10;

    public DestroyAfterTurns(int currentTurnCount, int destroyAfterCount)
    {
        CurrentTurnCount = currentTurnCount;
        DestroyAfterTurnCount = destroyAfterCount;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
            CurrentTurnCount++;
            if (CurrentTurnCount >= DestroyAfterTurnCount)
            {
                Spawner.Despawn(Self);
                Self.Destroyed(); //TODO: probably will need to use this callback in more places or something
            }
        }
    }
}

public class DTO_DestroyAfterTurns : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int current = 0;
        int destAfter = 0;
        string[] parameters = data.Split(',');
        foreach(string param in parameters)
        {
            string[] keyToValue = param.Split('=');
            switch(keyToValue[0])
            {
                case "CurrentTurnCount":
                    current = int.Parse(keyToValue[1]);
                    break;
                case "DestroyAfterTurnCount":
                    destAfter = int.Parse(keyToValue[1]);
                    break;
            }
        }
        Component = new DestroyAfterTurns(current, destAfter);
    }

    public string CreateSerializableData(IComponent component)
    {
        DestroyAfterTurns dafc = (DestroyAfterTurns)component;
        return $"{nameof(DestroyAfterTurns)}: CurrentTurnCount={dafc.CurrentTurnCount}, DestroyAfterTurnCount={dafc.DestroyAfterTurnCount}";
    }
}
