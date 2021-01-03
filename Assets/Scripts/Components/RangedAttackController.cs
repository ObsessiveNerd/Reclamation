using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : Component
{
    Point m_TileSelection;

    //Todo: I could probably initialize this with a specifc attack bow/spell/thrown object/whatever and get a lot of reuse out of this
    public RangedAttackController(IEntity self, Point startTileSelection)
    {
        Init(self);

        m_TileSelection = startTileSelection;

        RegisteredEvents.Add(GameEventId.UpdateEntity);
        RegisteredEvents.Add(GameEventId.HasInputController);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.HasInputController)
            gameEvent.Paramters[EventParameters.Value] = true;

        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = MoveDirection.None;

            if (Input.GetKeyDown(KeyCode.Keypad8))
                desiredDirection = MoveDirection.N;
            else if (Input.GetKeyDown(KeyCode.Keypad9))
                desiredDirection = MoveDirection.NE;
            else if (Input.GetKeyDown(KeyCode.Keypad6))
                desiredDirection = MoveDirection.E;
            else if (Input.GetKeyDown(KeyCode.Keypad3))
                desiredDirection = MoveDirection.SE;
            else if (Input.GetKeyDown(KeyCode.Keypad2))
                desiredDirection = MoveDirection.S;
            else if (Input.GetKeyDown(KeyCode.Keypad1))
                desiredDirection = MoveDirection.SW;
            else if (Input.GetKeyDown(KeyCode.Keypad4))
                desiredDirection = MoveDirection.W;
            else if (Input.GetKeyDown(KeyCode.Keypad7))
                desiredDirection = MoveDirection.NW;

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorld] = true;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInput(Self));
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
                gameEvent.Paramters[EventParameters.UpdateWorld] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            //Todo: implement actual call for attack and then use the energy like normal in the PlayerInput
            gameEvent.Paramters[EventParameters.TakeTurn] = false;
        }
    }
}
