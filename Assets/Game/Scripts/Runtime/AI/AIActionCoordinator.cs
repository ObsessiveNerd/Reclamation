using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAction
{

}

public class AIActionCoordinator : MonoBehaviour
{
    WeaponHandler m_Equipment;
    IAIAction m_Action;

    // Start is called before the first frame update
    void Start()
    {
        m_Equipment = GetComponentInChildren<WeaponHandler>();    
    }

    public IAIAction GetAction()
    {
        return null;
    }
}
