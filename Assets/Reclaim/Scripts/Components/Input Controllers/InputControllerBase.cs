using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputControllerBase : EntityComponent, IEscapeableMono
{
    public bool? AlternativeEscapeKeyPressed => false;

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
    }

    public void OnEscape()
    {
        UIManager.ForcePop(this);
    }

    protected void EndSelection(Point tileSelection)
    {
        Self.RemoveComponent(this);
        Self.AddComponent(new PlayerInputController());
        Services.TileSelectionService.EndTileSelection(tileSelection);
        Services.WorldUpdateService.UpdateWorldView();
        UIManager.ForcePop(this);
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
