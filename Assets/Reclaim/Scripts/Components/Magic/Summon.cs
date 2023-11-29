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
            IEntity spellSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            IEntity spellTarget = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Target));

            var validPoints = Services.DungeonService.GetValidPointsAround(Services.EntityMapService.GetPointWhereEntityIs(spellTarget), 2);

            for(int i = 0; i < Amount; i++)
            {
                IEntity e = EntityFactory.CreateEntity(SummonName);
                if(spellSource.HasComponent(typeof(Faction)))
                {
                    FactionId factionId = spellSource.GetComponent<Faction>().ID;
                    GameEvent setFaction = GameEventPool.Get(GameEventId.SetFaction)
                                            .With(EventParameter.Faction, factionId);
                    e.FireEvent(setFaction);
                }

                if (!e.HasComponent(typeof(PackTactics)))
                    e.AddComponent(new PackTactics(spellSource.ID));
                else
                    e.GetComponent<PackTactics>().PartyLeaderId = spellSource.ID;

                e.AddComponent(new DestroyAfterTurns(0, 8, true));

                GameEvent skipTurn = GameEventPool.Get(GameEventId.SkipTurn);
                if (validPoints.Count == 0)
                    break;

                var point = validPoints[0];

                GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                    .With(EventParameter.Entity, WorldUtility.GetGameObject(spellSource).transform.position)
                    .With(EventParameter.Target, WorldUtility.GetGameObject(Services.WorldDataQuery.GetEntityOnTile(point)).transform.position);
                Self.FireEvent(fireRangedWeapon).Release();
                fireRangedWeapon.Release();

                Spawner.Spawn(e, point);
                
                e.FireEvent(skipTurn);
                skipTurn.Release();
                validPoints.RemoveAt(0);
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
