using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
        //RegisteredEvents.Add(GameEventId.GetSprite);
        //RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        bool energyUsed = false;
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                FireEvent(Self, GameEventPool.Get(GameEventId.MoveKeyPressed)
                    .With(EventParameters.InputDirection, desiredDirection), true).Release();
                GameEvent setCamera = GameEventPool.Get(GameEventId.SetCameraPosition)
                                    .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(Self));
                FireEvent(World.Instance.Self, setCamera).Release();
                energyUsed = true;
            }

            else if (InputBinder.PerformRequestedAction(RequestedAction.OpenInventory))
            {
                FireEvent(Self, GameEventPool.Get(GameEventId.OpenInventory)).Release();
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerUIController());
            }

#if UNITY_EDITOR
            else if (Input.GetKeyDown(KeyCode.M))
            {
                FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.RevealAllTiles));
            }
#endif
            else if (InputBinder.PerformRequestedAction(RequestedAction.FireRangedWeapon))
            {
                GameEvent getRangedWeapon = FireEvent(Self, GameEventPool.Get(GameEventId.GetRangedWeapon)
                    .With(EventParameters.Value, null));
                IEntity rangedWeapon = EntityQuery.GetEntity((string)getRangedWeapon.Paramters[EventParameters.Value]);
                if (rangedWeapon != null)
                {
                    Self.RemoveComponent(this);
                    Self.AddComponent(new RangedPlayerAttackController(rangedWeapon));
                    gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                    //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
                }
                else
                    RecLog.Log("No ranged weapon equiped");
                getRangedWeapon.Release();
                energyUsed = true;
            }
#if UNITY_EDITOR
            else if (Input.GetKeyDown(KeyCode.P))
            {
                GameEvent giveExp = GameEventPool.Get(GameEventId.GainExperience)
                                    .With(EventParameters.Exp, 10);
                Self.FireEvent(giveExp).Release();
            }

            else if (Input.GetKeyDown(KeyCode.O))
            {
                GameEvent takeDamage = GameEventPool.Get(GameEventId.TakeDamage)
                                    .With(EventParameters.DamageList, new List<Damage>() { new Damage(10, DamageType.Slashing) })
                                    .With(EventParameters.RollToHit, 20)
                                    .With(EventParameters.DamageSource, Self.ID);
                Self.FireEvent(takeDamage).Release();
            }
#endif
            else if (InputBinder.PerformRequestedAction(RequestedAction.Look))
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

            //else if (Input.GetKeyDown(KeyCode.G))
            //{
            //    FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.Pickup, new .With(EventParameters.Entity, Self.ID)));
            //    gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            //}

            else if (SpellSelected(out int spellIndex))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new SpellcasterPlayerController(spellIndex));
            }

            else if (InputBinder.PerformRequestedAction(RequestedAction.PickupItem))
            {
                GameEvent pickupItems = GameEventPool.Get(GameEventId.Pickup)
                                            .With(EventParameters.Entity, Self.ID);

                FireEvent(World.Instance.Self, pickupItems).Release();
                energyUsed = true;

                //var getInteractableObjectsPositions = FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.GetInteractableObjects, new .With(EventParameters.Value, new List<Point>())));
                //var result = (List<Point>)getInteractableObjectsPositions.Paramters[EventParameters.Value];
                //if (result.Count == 0)
                //    return;
                //if(result.Count == 1)
                //    FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.Interact, new .With(EventParameters.Entity, Self.ID),
                //                                                                        new .With(EventParameters.TilePosition, result[0])));
                //else
                //{
                //    Self.RemoveComponent(this);
                //    Self.AddComponent(new PromptForDirectionController((dir) =>
                //    {
                //        Self.RemoveComponent(typeof(PromptForDirectionController));
                //        Self.AddComponent(new PlayerInputController());
                //        FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.InteractInDirection, new .With(EventParameters.Entity, Self.ID),
                //                                                                                        new .With(EventParameters.InputDirection, dir)));
                //    }));
                //}
            }

            else if (InputBinder.PerformRequestedAction(RequestedAction.RotateCharacter))
            {
                RotateActiveCharacter(gameEvent);
                return;
            }

            else if (Input.GetKeyDown(KeyCode.Escape))
                GameObject.FindObjectOfType<SaveSystem>().Save();
            ///

            if (energyUsed)
            {
                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn)
                    .With(EventParameters.TakeTurn, false);
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
                checkForEnergy.Release();
            }
        }

        //Just for testing
        else if (gameEvent.ID == GameEventId.GetSprite)
        {
            //Sprite sprite = Resources.Load<Sprite>("Textures/Characters/active_dwarf");
            //gameEvent.Paramters[EventParameters.RenderSprite] = sprite;
        }

        else if(gameEvent.ID == GameEventId.AlterSprite)
        {

        }
    }

    void RotateActiveCharacter(GameEvent gameEvent)
    {
        FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.RotateActiveCharacter));
        gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
        gameEvent.ContinueProcessing = false;
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