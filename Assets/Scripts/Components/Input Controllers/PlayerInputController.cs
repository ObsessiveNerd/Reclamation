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
            {
                FireEvent(Self, new GameEvent(GameEventId.OpenInventory));
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerUIController());
            }

            else if(Input.GetKeyDown(KeyCode.M))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.RevealAllTiles));
            }

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

            else if (Input.GetKeyDown(KeyCode.G))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Pickup, new KeyValuePair<string, object>(EventParameters.Entity, Self)));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            else if (Input.GetKeyDown(KeyCode.C))
            {
                //Cast spell
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {

                var getInteractableObjectsPositions = FireEvent(World.Instance.Self, new GameEvent(GameEventId.GetInteractableObjects, new KeyValuePair<string, object>(EventParameters.Value, new List<Point>())));
                var result = (List<Point>)getInteractableObjectsPositions.Paramters[EventParameters.Value];
                if (result.Count == 0)
                    return;
                if(result.Count == 1)
                    FireEvent(World.Instance.Self, new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                        new KeyValuePair<string, object>(EventParameters.TilePosition, result[0])));
                else
                {
                    Self.RemoveComponent(this);
                    Self.AddComponent(new PromptForDirectionController((dir) =>
                    {
                        Self.RemoveComponent(typeof(PromptForDirectionController));
                        Self.AddComponent(new PlayerInputController());
                        FireEvent(World.Instance.Self, new GameEvent(GameEventId.InteractInDirection, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                                        new KeyValuePair<string, object>(EventParameters.InputDirection, dir)));
                    }));
                }
            }

            //This is temporary, we can keep this functionality but right now it's just to test dropping items from your bag
            else if (Input.GetKeyDown(KeyCode.D))
                FireEvent(Self, new GameEvent(GameEventId.EmptyBag));

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.RotateActiveCharacter));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                gameEvent.ContinueProcessing = false;
                return;
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