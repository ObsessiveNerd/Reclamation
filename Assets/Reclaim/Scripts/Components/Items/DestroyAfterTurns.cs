using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTurns : EntityComponent
{
    public int CurrentTurnCount;
    public int DestroyAfterTurnCount;
    public bool Active;

    public override int Priority => 10;

    public DestroyAfterTurns(int currentTurnCount, int destroyAfterCount, bool isActive = false)
    {
        CurrentTurnCount = currentTurnCount;
        DestroyAfterTurnCount = destroyAfterCount;
        Active = isActive;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.ActivateObject);
        RegisteredEvents.Add(GameEventId.DeactivateObject);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.ActivateObject)
            Active = true;

        if (gameEvent.ID == GameEventId.DeactivateObject)
            Active = false;

        if (gameEvent.ID == GameEventId.EndTurn)
        {
            if(Active)
                CurrentTurnCount++;

            if (CurrentTurnCount >= DestroyAfterTurnCount)
            {
                Spawner.Despawn(Self);
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
        bool active = false;
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
                case "Active":
                    active = bool.Parse(keyToValue[1]);
                    break;
            }
        }
        Component = new DestroyAfterTurns(current, destAfter, active);
    }

    public string CreateSerializableData(IComponent component)
    {
        DestroyAfterTurns dafc = (DestroyAfterTurns)component;
        return $"{nameof(DestroyAfterTurns)}: CurrentTurnCount={dafc.CurrentTurnCount}, DestroyAfterTurnCount={dafc.DestroyAfterTurnCount}, Active={dafc.Active}";
    }
}
