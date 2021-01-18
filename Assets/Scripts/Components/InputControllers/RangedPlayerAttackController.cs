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
        GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.Target, WorldUtility.GetClosestEnemyTo(Self)),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
        FireEvent(World.Instance.Self, selectTile);
        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                TypeWeapon weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(Self, m_TileSelection);

                CombatUtility.Attack(Self, target, m_Attack);

                EndSelection(gameEvent);

                GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(gameEvent);
        }
    }

    void EndSelection(GameEvent gameEvent)
    {
        Self.RemoveComponent(this);
        Self.AddComponent(new PlayerInputController());
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
        gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
        //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
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
