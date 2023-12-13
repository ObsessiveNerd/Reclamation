using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilities : EntityComponent
{
    public List<GameObject> ActiveAbilitiesList = new List<GameObject>();

    public ActiveAbilities(List<GameObject> entities)
    {
        ActiveAbilitiesList = entities;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.AddToActiveAbilities);
        RegisteredEvents.Add(GameEventId.RemoveFromActiveAbilities);
        RegisteredEvents.Add(GameEventId.GetActiveAbilities);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddToActiveAbilities)
        {
            GameObject ability = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            if (ActiveAbilitiesList.Contains(ability))
                ActiveAbilitiesList.Remove(ability);
            else
                ActiveAbilitiesList.Add(ability);
        }
        else if(gameEvent.ID == GameEventId.GetActiveAbilities)
        {
            gameEvent.GetValue<List<GameObject>>(EventParameter.Abilities).AddRange(ActiveAbilitiesList);
        }
        else if(gameEvent.ID == GameEventId.RemoveFromActiveAbilities)
        {
            foreach(var entity in gameEvent.GetValue<List<GameObject>>(EventParameter.Abilities))
            {
                if(ActiveAbilitiesList.Contains(entity))
                    ActiveAbilitiesList.Remove(entity);
            }
        }
    }
}

public class DTO_ActiveAbilities : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var list = EntityFactory.GetEntitiesFromArray(data.Split('=')[1]);
        Component = new ActiveAbilities(list);
    }

    public string CreateSerializableData(IComponent component)
    {
        ActiveAbilities aa = (ActiveAbilities)component;
        string array = EntityFactory.ConvertEntitiesToStringArrayWithId(aa.ActiveAbilitiesList);
        return $"{nameof(ActiveAbilities)}: {nameof(aa.ActiveAbilitiesList)}={array}";
    }
}
