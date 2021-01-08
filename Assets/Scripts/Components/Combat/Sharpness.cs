using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sharpness : Component
{
    public Sharpness(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.Attack);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        int rollToHit = Dice.Roll("1d20");
        if (gameEvent.Paramters.ContainsKey(EventParameters.RollToHit))
            rollToHit = (int)gameEvent.Paramters[EventParameters.RollToHit];

        GameEvent sharpness = new GameEvent(GameEventId.Sharpness, gameEvent.Paramters);
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.ApplyEventToTile, new KeyValuePair<string, object>(EventParameters.Value, sharpness),
                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, gameEvent.Paramters[EventParameters.TilePosition])));
    }
}
