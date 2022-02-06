using System;
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

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                    .With(EventParameters.Abilities, new List<IEntity>());

        var eventResult = Self.FireEvent(getSpells);
        var spellList = eventResult.GetValue<List<IEntity>>(EventParameters.Abilities);
        if(spellList.Count == 0)
        {
            Self.RemoveComponent(this);
            Self.AddComponent(new PlayerInputController());
            getSpells.Release();
            return;
        }

        if(spellList.ToList().Count() > m_SpellIndex)
        {
            m_Attack = spellList[m_SpellIndex];

            GameEvent getCurrentMana = GameEventPool.Get(GameEventId.GetMana)
                                            .With(EventParameters.Value, 0);
            int currentMana = Self.FireEvent(getCurrentMana).GetValue<int>(EventParameters.Value);

            GameEvent getManaCost = GameEventPool.Get(GameEventId.ManaCost)
                                        .With(EventParameters.Value, 1);
            m_ManaCost = m_Attack.FireEvent(getManaCost).GetValue<int>(EventParameters.Value);

            if (m_ManaCost <= currentMana)
                Debug.Log("Had enough mana");
            else
            {
                Debug.Log("not enough mana");
                m_Attack = null;
                getManaCost.Release();
                getCurrentMana.Release();
                getSpells.Release();
                return;
            }

            getManaCost.Release();
            getCurrentMana.Release();
            getSpells.Release();
        } 

        IEntity startingTarget = WorldUtility.GetClosestEnemyTo(Self);

        if (startingTarget != null)
        {
            //GameEvent isVisible = GameEventPool.Get(GameEventId.EntityVisibilityState)
            //                            .With(EventParameters.Entity, startingTarget.ID)
            //                            .With(EventParameters.Value, false);

            //Here we can check isVisible to see if the target is invisible or something
            //FireEvent(startingTarget, isVisible);

            GameEvent isInFOV = GameEventPool.Get(GameEventId.IsInFOV)
                                    .With(EventParameters.Entity, startingTarget.ID)
                                    .With(EventParameters.Value, false);

            bool isInFoVResult = FireEvent(Self, isInFOV).GetValue<bool>(EventParameters.Value);
            if (!isInFoVResult)
                startingTarget = Self;
            isInFOV.Release();
        }
        else
            startingTarget = Self;

        m_TileSelection = Services.WorldDataQuery.GetEntityLocation(startingTarget);
        GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile)
            .With(EventParameters.TilePosition, m_TileSelection);
        m_Attack.FireEvent(selectTile);
        Services.TileSelectionService.SelectTile(m_TileSelection);
        Services.WorldUpdateService.UpdateWorldView();
        selectTile.Release();
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            if(m_Attack == null)
            {
                EndSelection(m_TileSelection);
                return;
            }

            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = GameEventPool.Get(GameEventId.SelectNewTileInDirection).With(EventParameters.InputDirection, desiredDirection)
                                                                                                    .With(EventParameters.TilePosition, m_TileSelection);
                moveSelection.Paramters[EventParameters.TilePosition] = Services.TileSelectionService.SelectTileInNewDirection(m_TileSelection, desiredDirection);
                FireEvent(m_Attack, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                Services.WorldUpdateService.UpdateWorldView();
                moveSelection.Release();
            }

            if(Input.GetKeyDown(KeyCode.Return) || SpellSelected(out int spell) && spell == m_SpellIndex)
            {
                //AttackType weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(m_TileSelection);

                CombatUtility.CastSpell(Self, target, m_Attack);
                //CombatUtility.Attack(Self, target, m_Attack, weaponType);

                EndSelection(m_TileSelection);

                GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameters.TilePosition, m_TileSelection);
                m_Attack.FireEvent(eb).Release();

                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn).With(EventParameters.TakeTurn, false);
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
                checkForEnergy.Release();

                //GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, m_ManaCost);
                //FireEvent(Self, depleteMana).Release();

                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            { 
                GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameters.TilePosition, m_TileSelection);
                m_Attack.FireEvent(eb).Release();
                EndSelection(m_TileSelection);
            }
        }
    }
}
