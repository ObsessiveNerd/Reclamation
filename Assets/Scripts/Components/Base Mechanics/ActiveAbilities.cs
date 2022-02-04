using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilities : EntityComponent
{
    public List<IEntity> ActiveAbilitiesList = new List<IEntity>();

    public ActiveAbilities(List<IEntity> entities)
    {
        ActiveAbilitiesList = entities;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AddToActiveAbilities);
        RegisteredEvents.Add(GameEventId.RemoveFromActiveAbilities);
        RegisteredEvents.Add(GameEventId.GetActiveAbilities);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddToActiveAbilities)
        {
            IEntity ability = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            if (ActiveAbilitiesList.Contains(ability))
                ActiveAbilitiesList.Remove(ability);
            else
                ActiveAbilitiesList.Add(ability);
        }
        else if(gameEvent.ID == GameEventId.GetActiveAbilities)
        {
            gameEvent.GetValue<List<IEntity>>(EventParameters.Abilities).AddRange(ActiveAbilitiesList);
        }
        else if(gameEvent.ID == GameEventId.RemoveFromActiveAbilities)
        {
            foreach(var entity in gameEvent.GetValue<List<IEntity>>(EventParameters.Abilities))
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
