using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    public PlayerInputController(IEntity self)
    {
        Init(self);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

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
                                                                                new KeyValuePair<string, object>(EventParameters.Target, World.Instance.GetClosestEnemyTo(Self)),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
                FireEvent(World.Instance.Self, selectTile);

                Self.AddComponent(new RangedAttackController(Self, (Point)selectTile.Paramters[EventParameters.TilePosition]));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if (Input.GetKeyDown(KeyCode.L))
            {
                Self.RemoveComponent(this);
                GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.Target, null),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
                FireEvent(World.Instance.Self, selectTile);

                Self.AddComponent(new LookController(Self, (Point)selectTile.Paramters[EventParameters.TilePosition]));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
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
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if(Input.GetKeyDown(KeyCode.Y))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PromptForDirectionController(Self));
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
            ///

            GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
            FireEvent(Self, checkForEnergy);
            gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
        }
    }
}
