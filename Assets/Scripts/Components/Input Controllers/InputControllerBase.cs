using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputControllerBase : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
    }

    protected void EndSelection(GameEvent gameEvent, Point tileSelection)
    {
        Self.RemoveComponent(this);
        Self.AddComponent(new PlayerInputController());
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, tileSelection)));
        gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
        //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
        UIManager.ForcePop();
    }

    protected bool SpellSelected(out int spell)
    {
        spell = 0;
        if(InputBinder.PerformRequestedAction(RequestedAction.SpellSelect1))
        {
            spell = 0;
            return true;
        }

        if(InputBinder.PerformRequestedAction(RequestedAction.SpellSelect2))
        {
            spell = 1;
            return true;
        }

        if(InputBinder.PerformRequestedAction(RequestedAction.SpellSelect3))
        {
            spell = 2;
            return true;
        }

        if(InputBinder.PerformRequestedAction(RequestedAction.SpellSelect4))
        {
            spell = 3;
            return true;
        }

        if(InputBinder.PerformRequestedAction(RequestedAction.SpellSelect5))
        {
            spell = 4;
            return true;
        }
        return false;
    }
}
