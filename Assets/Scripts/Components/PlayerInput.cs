using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : Component
{
    public PlayerInput(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
        RegisteredEvents.Add(GameEventId.HasInputController);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            //todo: refactor this block to use command pattern so we can re-bind keys
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
                FireEvent(Self, new GameEvent(GameEventId.MoveKeyPressed, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)));
            }

            else if (Input.GetKeyDown(KeyCode.I))
                FireEvent(Self, new GameEvent(GameEventId.OpenInventory));

            else if (Input.GetKeyDown(KeyCode.F))
            {
                Self.RemoveComponent(this);
                GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
                FireEvent(World.Instance.Self, selectTile);

                Self.AddComponent(new RangedAttackController(Self, (Point)selectTile.Paramters[EventParameters.TilePosition]));
                gameEvent.Paramters[EventParameters.UpdateWorld] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if (Input.GetKeyDown(KeyCode.Space))
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, Self)));

            //This is temporary, we can keep this functionality but right now it's just to test dropping items from your bag
            else if (Input.GetKeyDown(KeyCode.D))
                FireEvent(Self, new GameEvent(GameEventId.EmptyBag));

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.RotateActiveCharacter));
                gameEvent.Paramters[EventParameters.UpdateWorld] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
            ///

            GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
            FireEvent(Self, checkForEnergy);
            gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
        }

        if (gameEvent.ID == GameEventId.HasInputController)
            gameEvent.Paramters[EventParameters.Value] = true;
    }
}
