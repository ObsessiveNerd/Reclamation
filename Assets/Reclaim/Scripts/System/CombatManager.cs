using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat
{
    public List<GameObject> CombatQueue = new List<GameObject>();
}

//TODO
public class CombatManager : MonoBehaviour
{
    Dictionary<GameObject, Combat> m_CombatMap = new Dictionary<GameObject, Combat>();

    public void AddToCombat(GameObject source, GameObject target)
    {
        if(m_CombatMap.ContainsKey(target))
        {
            m_CombatMap[target].CombatQueue.Add(source);
        }
        else
        {
            Combat newCombat = new Combat();
            newCombat.CombatQueue.Add(source);
            newCombat.CombatQueue.Add(target);

            m_CombatMap[target] = newCombat;
            m_CombatMap[source] = newCombat;
        }
    }
}
