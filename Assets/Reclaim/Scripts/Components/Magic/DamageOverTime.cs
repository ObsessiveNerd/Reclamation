using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : EntityComponent
{
    public int CurrentTurnCount;
    public int DestroyAfterTurnCount;
    public string DiceNotation;

    Dice m_Dice;

    public DamageOverTime(string dice, int currentTurn, int destroyAfterTurns)
    {
        DiceNotation = dice;
        CurrentTurnCount = currentTurn;
        DestroyAfterTurnCount = destroyAfterTurns;
        m_Dice = new Dice(dice);
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
            CurrentTurnCount++;

            if (CurrentTurnCount >= DestroyAfterTurnCount)
            {
                Self.RemoveComponent(this);
            }

            else if(CurrentTurnCount % 2 == 0)
            {
                GameEvent dealDamage = GameEventPool.Get(GameEventId.TakeDamage)
                                        .With(EventParameter.DamageList, new List<Damage>()
                                        {
                                            new Damage(m_Dice.Roll(), DamageType.Poison)
                                        })
                                        .With(EventParameter.DamageSource, Self.ID)
                                        .With(EventParameter.Attack, Self.ID);
                Self.FireEvent(dealDamage).Release();
            }
        }
    }
}

public class DTO_DamageOverTime : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string dice = "";
        int currentturn = 0;
        int destroyafterturns = 0;

        string[] kvpairs = data.Split(',');
        foreach(var keyvaluepair in kvpairs)
        {
            string key = keyvaluepair.Split('=')[0];
            string value = keyvaluepair.Split('=')[1];

            switch (key)
            {
                case "DiceNotation":
                    dice = value;
                    break;
                case "DestroyAfterTurnCount":
                    destroyafterturns = int.Parse(value);
                    break;
                case "CurrentTurnCount":
                    currentturn = int.Parse(value);
                    break;
            }
        }
        Component = new DamageOverTime(dice, currentturn, destroyafterturns);
    }

    public string CreateSerializableData(IComponent component)
    {
        DamageOverTime bat = (DamageOverTime)component;
        return $"{nameof(DamageOverTime)}: {nameof(bat.DiceNotation)}={bat.DiceNotation}, {nameof(bat.DestroyAfterTurnCount)}={bat.DestroyAfterTurnCount}, {nameof(bat.CurrentTurnCount)}={bat.CurrentTurnCount}";
    }
}
