using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAction
{

}

public class AIActionCoordinator : MonoBehaviour
{
    MeleeArea m_Equipment;
    IAIAction m_Action;

    // Start is called before the first frame update
    void Start()
    {
        m_Equipment = GetComponentInChildren<MeleeArea>();    
    }

    public IAIAction GetAction()
    {
        return null;
    }
}
