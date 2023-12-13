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
    GameObject m_Attack;

    public SpellcasterPlayerController(int spellIndex)
    {
        m_SpellIndex = spellIndex;
    }

    public SpellcasterPlayerController(GameObject spell)
    {
        m_Attack = spell;
        GameEvent getManaCost = GameEventPool.Get(GameEventId.ManaCost)
                                        .With(EventParameter.Value, 1);
        m_ManaCost = m_Attack.FireEvent(getManaCost).GetValue<int>(EventParameter.Value);
        getManaCost.Release();
    }

    public void Start()
    {
        
        UIManager.Push(this);

        if(m_Attack == null)
        {
            GameEvent getSpells = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                    .With(EventParameter.Abilities, new List<GameObject>());

            var eventResult = Self.FireEvent(getSpells);
            var spellList = eventResult.GetValue<List<GameObject>>(EventParameter.Abilities);
            if (spellList.Count == 0 || spellList.Count - 1 < m_SpellIndex)
            {
                UIManager.ForcePop(this);
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
                getSpells.Release();
                return;
            }

            if (spellList.ToList().Count() > m_SpellIndex)
            {
                m_Attack = spellList[m_SpellIndex];

                GameEvent getCurrentMana = GameEventPool.Get(GameEventId.GetMana)
                                            .With(EventParameter.Value, 0);
                int currentMana = Self.FireEvent(getCurrentMana).GetValue<int>(EventParameter.Value);

                GameEvent getManaCost = GameEventPool.Get(GameEventId.ManaCost)
                                        .With(EventParameter.Value, 1);
                m_ManaCost = m_Attack.FireEvent(getManaCost).GetValue<int>(EventParameter.Value);

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
        }

        GameObject startingTarget = WorldUtility.GetClosestEnemyTo(Self);
        if (m_Attack.HasComponent(typeof(Heal)))
            startingTarget = Self;

        else if (startingTarget != null)
        {
            GameEvent isInFOV = GameEventPool.Get(GameEventId.IsInFOV)
                                    .With(EventParameter.Entity, startingTarget.ID)
                                    .With(EventParameter.Value, false);

            bool isInFoVResult = FireEvent(Self, isInFOV).GetValue<bool>(EventParameter.Value);
            if (!isInFoVResult)
                startingTarget = Self;
            isInFOV.Release();
        }
        else
            startingTarget = Self;

        m_TileSelection = Services.WorldDataQuery.GetEntityLocation(startingTarget);
        
        Services.TileSelectionService.SelectTile(m_TileSelection);
         GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile)
                                .With(EventParameter.InputDirection, PathfindingUtility.GetDirectionTo(Services.EntityMapService.GetPointWhereEntityIs(Self), m_TileSelection))
                                .With(EventParameter.Source, Self.ID)
                                .With(EventParameter.TilePosition, m_TileSelection);
        m_Attack.FireEvent(selectTile).Release();
        Services.WorldUpdateService.UpdateWorldView();
        
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

            if(m_Attack.HasComponent(typeof(SelfTargetingSpell)))
            { 
                ConfirmSpellCast(gameEvent);
                return;
            }

            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = GameEventPool.Get(GameEventId.SelectNewTileInDirection).With(EventParameter.InputDirection, desiredDirection)
                                                                                                    .With(EventParameter.Source, Self.ID)
                                                                                                    .With(EventParameter.TilePosition, 
                                                                                                    Services.TileSelectionService.GetTilePointInDirection(m_TileSelection, desiredDirection));
                Services.TileSelectionService.SelectTileInNewDirection(m_TileSelection, desiredDirection);
                FireEvent(m_Attack, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameter.TilePosition];
                Services.WorldUpdateService.UpdateWorldView();
                moveSelection.Release();
            }

            if(Input.GetKeyDown(KeyCode.Return) || SpellSelected(out int spell) && spell == m_SpellIndex)
            {
                ConfirmSpellCast(gameEvent);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            { 
                GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameter.TilePosition, m_TileSelection);
                m_Attack.FireEvent(eb).Release();
                EndSelection(m_TileSelection);
            }
        }
    }

    void ConfirmSpellCast(GameEvent gameEvent)
    {
        //AttackType weaponType = CombatUtility.GetWeaponType(m_Attack);
        GameObject target = WorldUtility.GetEntityAtPosition(m_TileSelection);

        CombatUtility.Attack(Self, target, m_Attack);
        //CombatUtility.Attack(Self, target, m_Attack, weaponType);

        EndSelection(m_TileSelection);

        GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameter.TilePosition, m_TileSelection);
        m_Attack.FireEvent(eb).Release();

        GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn).With(EventParameter.TakeTurn, false);
        FireEvent(Self, checkForEnergy);
        gameEvent.Paramters[EventParameter.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameter.TakeTurn];
        checkForEnergy.Release();

        //GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, m_ManaCost);
        //FireEvent(Self, depleteMana).Release();
    }
}
