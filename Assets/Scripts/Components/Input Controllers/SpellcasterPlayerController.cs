using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellcasterPlayerController : InputControllerBase
{
    int m_SpellIndex = 0;
    int m_ManaCost = 0;
    Point m_TileSelection = Point.InvalidPoint;
    IEntity m_Attack;

    public SpellcasterPlayerController(int spellIndex)
    {
        m_SpellIndex = spellIndex;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);

        EventBuilder getSpells = new EventBuilder(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, new HashSet<string>());

        var eventResult = Self.FireEvent(getSpells.CreateEvent());
        var spellList = eventResult.GetValue<HashSet<string>>(EventParameters.SpellList);
        if(spellList.ToList().Count() > m_SpellIndex)
        {
            m_Attack = EntityQuery.GetEntity(spellList.ToList()[m_SpellIndex]);

            EventBuilder getCurrentMana = new EventBuilder(GameEventId.GetMana)
                                            .With(EventParameters.Value, 0);
            int currentMana = Self.FireEvent(getCurrentMana.CreateEvent()).GetValue<int>(EventParameters.Value);

            EventBuilder getManaCost = new EventBuilder(GameEventId.ManaCost)
                                        .With(EventParameters.Value, 1);
            m_ManaCost = m_Attack.FireEvent(getManaCost.CreateEvent()).GetValue<int>(EventParameters.Value);

            if (m_ManaCost <= currentMana)
                Debug.Log("Had enough mana");
            else
            {
                Debug.Log("not enough mana");
                m_Attack = null;
            }
        } 

        IEntity startingTarget = WorldUtility.GetClosestEnemyTo(Self);

        if (startingTarget != null)
        {
            EventBuilder isVisible = new EventBuilder(GameEventId.EntityVisibilityState)
                                        .With(EventParameters.Entity, startingTarget.ID)
                                        .With(EventParameters.Value, false);

            //Here we can check isVisible to see if the target is invisible or something
            //FireEvent(startingTarget, isVisible.CreateEvent());

            EventBuilder isInFOV = new EventBuilder(GameEventId.IsInFOV)
                                    .With(EventParameters.Entity, startingTarget.ID)
                                    .With(EventParameters.Value, false);

            bool isInFoVResult = FireEvent(Self, isInFOV.CreateEvent()).GetValue<bool>(EventParameters.Value);
            if (!isInFoVResult)
                startingTarget = Self;
        }
        else
            startingTarget = Self;

        GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID),
                                                                                new KeyValuePair<string, object>(EventParameters.Target, startingTarget.ID),
                                                                                new KeyValuePair<string, object>(EventParameters.TilePosition, null));
        FireEvent(World.Instance.Self, selectTile);
        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.UpdateWorldView));
        UIManager.Push(null);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            if(m_Attack == null)
            {
                EndSelection(gameEvent, m_TileSelection);
                return;
            }

            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            if(Input.GetKeyDown(KeyCode.Return) || SpellSelected(out int spell) && spell == m_SpellIndex)
            {
                TypeWeapon weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(m_TileSelection);

                CombatUtility.Attack(Self, target, m_Attack);

                EventBuilder fireRangedWeapon = new EventBuilder(GameEventId.FireRangedAttack)
                                                .With(EventParameters.Entity, WorldUtility.GetGameObject(Self).transform.position)
                                                .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                FireEvent(m_Attack, fireRangedWeapon.CreateEvent());

                EndSelection(gameEvent, m_TileSelection);

                GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];

                GameEvent depleteMana = new GameEvent(GameEventId.DepleteMana, new KeyValuePair<string, object>(EventParameters.Mana, m_ManaCost));
                FireEvent(Self, depleteMana);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(gameEvent, m_TileSelection);
        }
    }
}
