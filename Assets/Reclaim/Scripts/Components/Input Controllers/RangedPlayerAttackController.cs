using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPlayerAttackController : InputControllerBase
{
    Point m_TileSelection;
    IEntity m_Attack;

    public RangedPlayerAttackController(IEntity attack)
    {
        m_Attack = attack;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);

        IEntity startingTarget = WorldUtility.GetClosestEnemyTo(Self);

        //GameEvent isVisible = GameEventPool.Get(GameEventId.EntityVisibilityState)
        //                            .With(EventParameters.Entity, startingTarget.ID)
        //                            .With(EventParameters.Value, false);

        //Here we can check isVisible to see if the target is invisible or something
        //FireEvent(startingTarget, isVisible.CreateEvent());

        GameEvent isInFOV = GameEventPool.Get(GameEventId.IsInFOV)
                                .With(EventParameter.Entity, startingTarget.ID)
                                .With(EventParameter.Value, false);

        bool isInFoVResult = FireEvent(Self, isInFOV).GetValue<bool>(EventParameter.Value);
        if (!isInFoVResult)
            startingTarget = Self;
        isInFOV.Release();

        m_TileSelection = Services.WorldDataQuery.GetEntityLocation(startingTarget);
        Services.TileSelectionService.SelectTile(m_TileSelection);
        Services.WorldUpdateService.UpdateWorldView();
        UIManager.Push(this);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            if (m_Attack == null)
            {
                EndSelection(m_TileSelection);
                return;
            }

            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                m_TileSelection = Services.TileSelectionService.SelectTileInNewDirection(m_TileSelection, desiredDirection);
                Services.WorldUpdateService.UpdateWorldView();
            }

            if(Input.GetKeyDown(KeyCode.Return) || InputBinder.PerformRequestedAction(RequestedAction.FireRangedWeapon))
            {
                //AttackType weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(m_TileSelection);

                GameEvent getAmmo = GameEventPool.Get(GameEventId.GetAmmo)
                    .With(EventParameter.Value, null);

                IEntity attack = EntityQuery.GetEntity(FireEvent(m_Attack, getAmmo).GetValue<string>(EventParameter.Value));

                CombatUtility.Attack(Self, target, attack, AttackType.Ranged);

                EndSelection(m_TileSelection);

                //GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                //                                .With(EventParameters.Entity, WorldUtility.GetGameObject(Self).transform.position)
                //                                .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                //FireEvent(m_Attack, fireRangedWeapon.CreateEvent());

                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn)
                    .With(EventParameter.TakeTurn, false);
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameter.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameter.TakeTurn];
                checkForEnergy.Release();
                getAmmo.Release();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(m_TileSelection);
        }
    }
}

public class DTO_RangedPlayerAttackController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        IEntity attack = EntityFactory.CreateEntity(data);
        Component = new RangedPlayerAttackController(attack);
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerInputController);
    }
}
