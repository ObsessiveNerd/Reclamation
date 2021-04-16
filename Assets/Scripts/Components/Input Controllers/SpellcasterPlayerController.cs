using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellcasterPlayerController : InputControllerBase
{
    List<string> m_Spells = new List<string>();
    Point m_TileSelection = Point.InvalidPoint;
    int m_CurrentSpellIndex = 0;

    public override void Init(IEntity self)
    {
        base.Init(self);
        EventBuilder getSpells = new EventBuilder(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, m_Spells);

        FireEvent(Self, getSpells.CreateEvent());

        EventBuilder openSpellUI = new EventBuilder(GameEventId.OpenSpellUI)
                                    .With(EventParameters.Entity, Self.ID)
                                    .With(EventParameters.SpellList, m_Spells);

        FireEvent(World.Instance.Self, openSpellUI.CreateEvent());

        //Self.RemoveComponent(this);
        //Self.AddComponent(new PlayerUIController());

        RegisteredEvents.Add(GameEventId.SpellSelected);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            //MoveDirection desiredDirection = InputUtility.GetMoveDirection();
            //if (desiredDirection == MoveDirection.E)
            //{
            //    m_CurrentSpellIndex++;
            //    if (m_CurrentSpellIndex >= m_Spells.Count)
            //        m_CurrentSpellIndex = 0;
            //}
            //else if (desiredDirection == MoveDirection.W)
            //{
            //    m_CurrentSpellIndex--;
            //    if (m_CurrentSpellIndex < 0)
            //        m_CurrentSpellIndex = m_Spells.Count - 1;
            //}

            //if (Input.GetKeyDown(KeyCode.Return))
            //{
            //    //Todo: determine type of spell and perform appropriate actions.  For instance, ranged attacks work fine as shown below but things like "hold person" won't work that way.
            //    Self.RemoveComponent(this);
            //    Self.AddComponent(new RangedPlayerAttackController(EntityQuery.GetEntity(m_Spells[m_CurrentSpellIndex])));
            //}

            if (Input.GetKeyDown(KeyCode.Escape))
                EndSelection(gameEvent, m_TileSelection);
        }

        else if(gameEvent.ID == GameEventId.SpellSelected)
        {
            string spellId = gameEvent.GetValue<string>(EventParameters.Spell);
            Self.RemoveComponent(this);
            Self.AddComponent(new RangedPlayerAttackController(EntityQuery.GetEntity(spellId)));
        }
    }
}
