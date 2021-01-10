﻿using System.Collections;
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
                FireEvent(Self, new GameEvent(GameEventId.MoveKeyPressed, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)));

            else if (Input.GetKeyDown(KeyCode.I))
                FireEvent(Self, new GameEvent(GameEventId.OpenInventory));

            else if (Input.GetKeyDown(KeyCode.F))
            {
                GameEvent getRangedWeapon = FireEvent(Self, new GameEvent(GameEventId.GetRangedWeapon, new KeyValuePair<string, object>(EventParameters.Value, null)));
                IEntity rangedWeapon = (IEntity)getRangedWeapon.Paramters[EventParameters.Value];
                if (rangedWeapon != null)
                {
                    Self.RemoveComponent(this);
                    Self.AddComponent(new RangedPlayerAttackController(Self, rangedWeapon));
                    gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                    gameEvent.Paramters[EventParameters.CleanupComponents] = true;
                }
                else
                    RecLog.Log("No ranged weapon equiped");
            }

            else if (Input.GetKeyDown(KeyCode.L))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new LookController(Self));

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
