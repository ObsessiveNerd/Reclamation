using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
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
                    Self.AddComponent(new RangedPlayerAttackController(rangedWeapon));
                    gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                    //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
                }
                else
                    RecLog.Log("No ranged weapon equiped");
            }

            else if (Input.GetKeyDown(KeyCode.L))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new LookController());

                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if (Input.GetKeyDown(KeyCode.T))
            {
                //Throw an equiped weapon
            }

            else if (Input.GetKeyDown(KeyCode.C))
            {
                //Cast spell
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {
                //TODO: we need to query the world for nearby interactable objects, if there's more than 1 give those to the input selection controller with an event callback
                //If there's only 1 we can fire the event off directly to the target entity, which we should get back from our query
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, Self)));
            }

            //This is temporary, we can keep this functionality but right now it's just to test dropping items from your bag
            else if (Input.GetKeyDown(KeyCode.D))
                FireEvent(Self, new GameEvent(GameEventId.EmptyBag));

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.RotateActiveCharacter));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if (Input.GetKeyDown(KeyCode.Y))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PromptForDirectionController());
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            else if (Input.GetKeyDown(KeyCode.Escape))
                GameObject.FindObjectOfType<SaveSystem>().Save();
            ///

            GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
            FireEvent(Self, checkForEnergy);
            gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
        }
    }
}

public class DTO_PlayerInputController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PlayerInputController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerInputController);
    }
}