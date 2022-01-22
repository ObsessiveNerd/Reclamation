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
                                .With(EventParameters.Entity, startingTarget.ID)
                                .With(EventParameters.Value, false);

        bool isInFoVResult = FireEvent(Self, isInFOV).GetValue<bool>(EventParameters.Value);
        if (!isInFoVResult)
            startingTarget = Self;
        isInFOV.Release();

        Services.TileSelectionService.SelectTile(Services.WorldDataQuery.GetEntityLocation(startingTarget));
        Services.WorldUpdateService.UpdateWorldView();
        UIManager.Push(null);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            if (m_Attack == null)
            {
                EndSelection(gameEvent, m_TileSelection);
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
                TypeWeapon weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(m_TileSelection);

                CombatUtility.Attack(Self, target, m_Attack, false);

                EndSelection(gameEvent, m_TileSelection);

                //GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                //                                .With(EventParameters.Entity, WorldUtility.GetGameObject(Self).transform.position)
                //                                .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                //FireEvent(m_Attack, fireRangedWeapon.CreateEvent());

                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn)
                    .With(EventParameters.TakeTurn, false);
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
                checkForEnergy.Release();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(gameEvent, m_TileSelection);
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
