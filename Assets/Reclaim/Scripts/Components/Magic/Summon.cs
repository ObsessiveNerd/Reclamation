using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : EntityComponent
{
    public string SummonName;
    public int Amount;

    public Summon(string summonName, int amount)
    {
        SummonName = summonName;
        Amount = amount;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.CastSpellEffect)
        {
            IEntity spellSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            for(int i = 0; i < Amount; i++)
            {
                IEntity e = EntityFactory.CreateEntity(SummonName);
                if(spellSource.HasComponent(typeof(Faction)))
                {
                    FactionId factionId = spellSource.GetComponent<Faction>().ID;
                    GameEvent setFaction = GameEventPool.Get(GameEventId.SetFaction)
                                            .With(EventParameters.Faction, factionId);
                    e.FireEvent(setFaction);
                }

                var point = Services.DungeonService.GetRandomValidPointInSameRoom(Services.EntityMapService.GetPointWhereEntityIs(spellSource));
                Spawner.Spawn(e, point);
            }
        }
    }
}

public class DTO_Summon : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string summonName = "";
        int amount = 0;

        var kvpairs = data.Split(',');
        foreach(var kvp in kvpairs)
        {
            var key = kvp.Split('=')[0];
            var value = kvp.Split('=')[1];

            switch(key)
            {
                case "Amount":
                    amount = int.Parse(value);
                    break;
                case "SummonName":
                    summonName = value;
                    break;
            }
        }
        Component = new Summon(summonName, amount);
    }

    public string CreateSerializableData(IComponent component)
    {
        Summon summon = (Summon)component;
        return $"{nameof(Summon)}: {nameof(summon.Amount)}={summon.Amount},{nameof(summon.SummonName)}={summon.SummonName}";
    }
}
