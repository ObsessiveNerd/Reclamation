using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : InputControllerBase
{
    Point m_TileSelection;
    IEntity m_Attack;

    //Todo: I could probably initialize this with a specifc attack bow/spell/thrown object/whatever and get a lot of reuse out of this
    public RangedAttackController(IEntity self/*, IEntity attack*/)
    {
        Init(self);
        //m_Attack = attack;

        GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.Target, World.Instance.GetClosestEnemyTo(Self)),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
        FireEvent(World.Instance.Self, selectTile);
        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                //weapon here could also mean a spell, we also need to run the components on Self and ensure that there are no modifiers that need to apply.  Attack should come in as the base attack, no mods
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Attack, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection),
                                                                                    new KeyValuePair<string, object>(EventParameters.Attack, m_Attack)));
                EndSelection(gameEvent);

                //Very temporary, we need to use the energy costs and blah blah blah
                FireEvent(Self, new GameEvent(GameEventId.SkipTurn));
                gameEvent.Paramters[EventParameters.TakeTurn] = true;
                ////////////////////////////////////////////////////////////////////
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(gameEvent);
        }
    }

    void EndSelection(GameEvent gameEvent)
    {
        Self.RemoveComponent(this);
        Self.AddComponent(new PlayerInputController(Self));
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
        gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
        gameEvent.Paramters[EventParameters.CleanupComponents] = true;
    }
}
