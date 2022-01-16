using System.Collections.Generic;
using System.Linq;

public static class SpawnRestrictionTags
{
    public const string OnWall = nameof(OnWall);
    public const string AgainstWall = nameof(AgainstWall);
    public const string CenterRoom = nameof(CenterRoom);
    public const string AnywhereButWall = nameof(AnywhereButWall);
}

public class SpawnRestriction
{
    public HashSet<string> Restrictions = new HashSet<string>();

    public SpawnRestriction(params string[] args)
    {
        foreach (var s in args)
            Restrictions.Add(s);
    }

    public void Add(string restriction)
    {
        Restrictions.Add(restriction);
    }

    public void Remove(string restriction)
    {
        if (Restrictions.Contains(restriction))
            Restrictions.Remove(restriction);
    }

    public override string ToString()
    {
        return string.Join(",", Restrictions.ToArray());
    }
}

public class SpawnRestrictor : Component
{
    public SpawnRestriction Restrictions;

    public SpawnRestrictor(SpawnRestriction restrictions)
    {
        Restrictions = restrictions;
    }

    public override void Init(IEntity self)
    {
        RegisteredEvents.Add(GameEventId.GetSpawnRestrictions);
        base.Init(self);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetSpawnRestrictions)
        {
            var value = gameEvent.GetValue<HashSet<string>>(EventParameters.Restrictions);
            foreach (string r in Restrictions.Restrictions)
                value.Add(r);
        }
    }
}

public class DTO_SpawnRestrictor : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var values = data.Split('=')[1];
        var kvp = values.Split(',');
        Component = new SpawnRestrictor(new SpawnRestriction(kvp));
    }

    public string CreateSerializableData(IComponent component)
    {
        SpawnRestrictor sr = (SpawnRestrictor)component;

        return $"{nameof(SpawnRestrictor)}: {nameof(sr.Restrictions)}={sr.Restrictions}";
    }
}