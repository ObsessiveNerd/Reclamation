using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryMono : MonoBehaviour
{
    IEntity m_Source;
    public void Setup(IEntity source, List<IEntity> inventory)
    {
        m_Source = source;
    }
}
