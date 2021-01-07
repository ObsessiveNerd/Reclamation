using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    int m_AmountOfDice;
    int m_DAmount;
    int m_Modifiers = 0;

    public Dice(string diceNotation)
    {
        string[] firstSplit = diceNotation.Split('d');
        m_AmountOfDice = int.Parse(firstSplit[0]);

        string[] secondSplit = firstSplit[1].Split('+');
        m_DAmount = int.Parse(secondSplit[0]);
        if(secondSplit.Length == 2)
            m_Modifiers = int.Parse(secondSplit[1]);
    }

    public int Roll()
    {
        int total = RecRandom.Instance.GetRandomValue(m_AmountOfDice, m_AmountOfDice * m_DAmount);
        total += m_Modifiers;
        return total;
    }
}
