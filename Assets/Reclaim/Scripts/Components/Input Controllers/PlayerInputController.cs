﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    public void Start()
    {
        
        //RegisteredEvents.Add(GameEventId.UpdateEntity, UpdateEntity);

        //RegisteredEvents.Add(GameEventId.OpenUI);
        //RegisteredEvents.Add(GameEventId.GetSprite);
        //RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    void Update()
    {
        MoveDirection desiredDirection = InputUtility.GetMoveDirection();
        if (desiredDirection != MoveDirection.None)
        {
            gameObject.FireEvent(GameEventPool.Get(GameEventId.MoveKeyPressed)
                        .With(EventParameter.InputDirection, desiredDirection), true).Release();
        }
    }

//    public override void HandleEvent(GameEvent gameEvent)
//    {
//        bool energyUsed = false;
//        if (gameEvent.ID == GameEventId.UpdateEntity)
//        {
//            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

//            if (desiredDirection != MoveDirection.None)
//            {
//                FireEvent(Self, GameEventPool.Get(GameEventId.MoveKeyPressed)
//                    .With(EventParameter.InputDirection, desiredDirection), true).Release();
//                Services.CameraService.SetCameraPosition(PathfindingUtility.GetEntityLocation(Self));
//                energyUsed = true;
//            }

//            else if (InputBinder.PerformRequestedAction(RequestedAction.OpenInventory))
//            {
//                FireEvent(Self, GameEventPool.Get(GameEventId.OpenInventory)).Release();
//                Self.RemoveComponent(this);
//                Self.AddComponent(new PlayerUIController());
//            }

//#if UNITY_EDITOR
//            else if (Input.GetKeyDown(KeyCode.M))
//            {
//                Services.FOVService.RevealAllTiles();
//            }

//            else if (Input.GetKeyDown(KeyCode.RightControl))
//            {
//                Services.StateManagerService.GameOver(true);
//            }

//            else if (Input.GetKeyDown(KeyCode.RightAlt))
//            {
//                Services.StateManagerService.GameOver(false);
//            }

//            else if (Input.GetKeyDown(KeyCode.Alpha9))
//                Services.DungeonService.MoveDown();

//            else if (Input.GetKeyDown(KeyCode.Alpha8))
//                Services.DungeonService.MoveUp();

//#endif
//            else if(InputBinder.PerformRequestedAction(RequestedAction.CastSpell))
//            {
//                Self.RemoveComponent(this);
//                Self.AddComponent(new PlayerUIController());
//                Services.WorldUIService.OpenSelectAllSpellUI();
//            }

//            else if (InputBinder.PerformRequestedAction(RequestedAction.FireRangedWeapon))
//            {

//                GameEvent getRangedWeapon = FireEvent(Self, GameEventPool.Get(GameEventId.GetWeapon)
//                    .With(EventParameter.Weapon, new List<string>()));

//                List<string> weapons = getRangedWeapon.GetValue<List<string>>(EventParameter.Weapon);
//                if (weapons.Count == 0)
//                    return;

//                GameObject weapon = EntityQuery.GetEntity(weapons[0]);
//                if (weapon.HasComponent(typeof(Bow)))
//                {
//                    Self.RemoveComponent(this);
//                    Self.AddComponent(new RangedPlayerAttackController(weapon));
//                    gameEvent.Paramters[EventParameter.UpdateWorldView] = true;
//                }
//                else
//                    RecLog.Log("No ranged weapon equiped");
//                getRangedWeapon.Release();
//                //energyUsed = true;
//            }
//#if UNITY_EDITOR
//            else if (Input.GetKeyDown(KeyCode.P))
//            {
//                GameEvent giveExp = GameEventPool.Get(GameEventId.GainExperience)
//                                    .With(EventParameter.Exp, 100);
//                Self.FireEvent(giveExp).Release();
//                Services.WorldUIService.UpdateUI();
//            }

//            else if (Input.GetKeyDown(KeyCode.O))
//            {
//                GameEvent takeDamage = GameEventPool.Get(GameEventId.TakeDamage)
//                                    .With(EventParameter.Attack, Self.ID)
//                                    .With(EventParameter.DamageList, new List<Damage>() { new Damage(100, DamageType.Slashing) })
//                                    .With(EventParameter.RollToHit, 20)
//                                    .With(EventParameter.DamageSource, Self.ID);
//                Self.FireEvent(takeDamage).Release();
//            }

//            else if (Input.GetKeyDown(KeyCode.Period))
//            {
//                GameEvent gainMana = GameEventPool.Get(GameEventId.RestoreMana)
//                    .With(EventParameter.Mana, 1000);
//                FireEvent(Self, gainMana).Release();
//            }

//            else if (Input.GetKeyDown(KeyCode.Alpha0))
//                Services.WorldUIService.OpenDebugMenu();
//#endif
//            else if (InputBinder.PerformRequestedAction(RequestedAction.Look))
//            {
//                Self.RemoveComponent(this);
//                Self.AddComponent(new LookController());

//                gameEvent.Paramters[EventParameter.UpdateWorldView] = true;
//                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
//            }

//            else if (Input.GetKeyDown(KeyCode.T))
//            {
//                //Throw an equiped weapon
//            }

//            //else if (Input.GetKeyDown(KeyCode.G))
//            //{
//            //    FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.Pickup, new .With(EventParameters.Entity, Self.ID)));
//            //    gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
//            //}

//            else if (SpellSelected(out int spellIndex))
//            {
//                if (!Self.HasComponent(typeof(SpellcasterPlayerController), true))
//                {
//                    Self.RemoveComponent(this);
//                    Self.AddComponent(new SpellcasterPlayerController(spellIndex));
//                }
//            }

//            else if (InputBinder.PerformRequestedAction(RequestedAction.InteractWithCurrentTile))
//            {
//                Services.TileInteractionService.Pickup(Self);
//                energyUsed = true;

//                //var getInteractableObjectsPositions = FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.GetInteractableObjects, new .With(EventParameters.Value, new List<Point>())));
//                //var result = (List<Point>)getInteractableObjectsPositions.Paramters[EventParameters.Value];
//                //if (result.Count == 0)
//                //    return;
//                //if(result.Count == 1)
//                //    FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.Interact, new .With(EventParameters.Entity, Self.ID),
//                //                                                                        new .With(EventParameters.TilePosition, result[0])));
//                //else
//                //{
//                //    Self.RemoveComponent(this);
//                //    Self.AddComponent(new PromptForDirectionController((dir) =>
//                //    {
//                //        Self.RemoveComponent(typeof(PromptForDirectionController));
//                //        Self.AddComponent(new PlayerInputController());
//                //        FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.InteractInDirection, new .With(EventParameters.Entity, Self.ID),
//                //                                                                                        new .With(EventParameters.InputDirection, dir)));
//                //    }));
//                //}
//            }

//            else if (InputBinder.PerformRequestedAction(RequestedAction.RotateCharacter))
//            {
//                RotateActiveCharacter(gameEvent);
//                return;
//            }

//            else if (Input.GetKeyDown(KeyCode.Escape))
//                Services.SaveAndLoadService.Save();
//            ///

//            if (energyUsed)
//            {
//                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn)
//                    .With(EventParameter.TakeTurn, false);
//                FireEvent(Self, checkForEnergy);
//                gameEvent.Paramters[EventParameter.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameter.TakeTurn];
//                checkForEnergy.Release();
//            }
//        }

//        else if (gameEvent.ID == GameEventId.OpenUI)
//        {
//            Self.RemoveComponent(this);
//            Self.AddComponent(new PlayerUIController());
//        }


//        //Just for testing
//        else if (gameEvent.ID == GameEventId.GetSprite)
//        {
//            //Sprite sprite = Resources.Load<Sprite>("Textures/Characters/active_dwarf");
//            //gameEvent.Paramters[EventParameters.RenderSprite] = sprite;
//        }

//        else if(gameEvent.ID == GameEventId.AlterSprite)
//        {

//        }
//    }

//    void RotateActiveCharacter(GameEvent gameEvent)
//    {
//        Services.PlayerManagerService.RotateActiveCharacter();
//        Services.WorldUpdateService.UpdateWorldView();
//        gameEvent.ContinueProcessing = false;
//    }
}
