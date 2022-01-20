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

        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, new HashSet<string>());

        var eventResult = Self.FireEvent(getSpells);
        var spellList = eventResult.GetValue<HashSet<string>>(EventParameters.SpellList);
        if(spellList.ToList().Count() > m_SpellIndex)
        {
            m_Attack = EntityQuery.GetEntity(spellList.ToList()[m_SpellIndex]);

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
            }

            getManaCost.Release();
            getCurrentMana.Release();
            getSpells.Release();
        } 

        IEntity startingTarget = WorldUtility.GetClosestEnemyTo(Self);

        if (startingTarget != null)
        {
            GameEvent isVisible = GameEventPool.Get(GameEventId.EntityVisibilityState)
                                        .With(EventParameters.Entity, startingTarget.ID)
                                        .With(EventParameters.Value, false);

            //Here we can check isVisible to see if the target is invisible or something
            //FireEvent(startingTarget, isVisible.CreateEvent());

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

        GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile).With(EventParameters.Entity, Self.ID)
                                                                                .With(EventParameters.Target, startingTarget.ID)
                                                                                .With(EventParameters.TilePosition, null);
        FireEvent(World.Services.Self, selectTile);
        FireEvent(m_Attack, selectTile);
        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
        FireEvent(World.Services.Self, GameEventPool.Get(GameEventId.UpdateWorldView));
        UIManager.Push(null);
        selectTile.Release();
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
                GameEvent moveSelection = GameEventPool.Get(GameEventId.SelectNewTileInDirection).With(EventParameters.InputDirection, desiredDirection)
                                                                                                    .With(EventParameters.TilePosition, m_TileSelection);
                FireEvent(World.Services.Self, moveSelection);
                FireEvent(m_Attack, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                moveSelection.Release();
            }

            if(Input.GetKeyDown(KeyCode.Return) || SpellSelected(out int spell) && spell == m_SpellIndex)
            {
                TypeWeapon weaponType = CombatUtility.GetWeaponType(m_Attack);
                IEntity target = WorldUtility.GetEntityAtPosition(m_TileSelection);

                CombatUtility.Attack(Self, target, m_Attack, false);

                GameEvent affectArea = GameEventPool.Get(GameEventId.AffectArea)
                                            .With(EventParameters.Effect, new Action<IEntity>((t) => 
                                                CombatUtility.Attack(Self, t, m_Attack, false, false)));
                m_Attack.FireEvent(affectArea).Release();

                //GameEvent fireRangedWeapon = GameEventPool.Get(GameEventId.FireRangedAttack)
                //                                .With(EventParameters.Entity, WorldUtility.GetGameObject(Self).transform.position)
                //                                .With(EventParameters.Target, WorldUtility.GetGameObject(target).transform.position);
                //FireEvent(m_Attack, fireRangedWeapon.CreateEvent());

                EndSelection(gameEvent, m_TileSelection);

                GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameters.TilePosition, m_TileSelection);
                m_Attack.FireEvent(eb).Release();

                GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn).With(EventParameters.TakeTurn, false);
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
                checkForEnergy.Release();

                GameEvent depleteMana = GameEventPool.Get(GameEventId.DepleteMana).With(EventParameters.Mana, m_ManaCost);
                FireEvent(Self, depleteMana).Release();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            { 
                EndSelection(gameEvent, m_TileSelection);
                GameEvent eb = GameEventPool.Get(GameEventId.EndSelection)
                                    .With(EventParameters.TilePosition, m_TileSelection);
                m_Attack.FireEvent(eb).Release();
            }
        }
    }
}
