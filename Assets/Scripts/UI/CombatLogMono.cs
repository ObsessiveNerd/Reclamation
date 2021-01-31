using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CombatLogMono : MonoBehaviour
{
    public GameObject CombatLog;

    void Start()
    {
        RecLog.MessageLogged += LogMessage;
    }

    void LogMessage(string message)
    {
        
    }

    int m_DebugIndex = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            CombatLog.SetActive(!CombatLog.activeSelf);

        if (Input.GetKeyDown(KeyCode.F2))
            RecLog.Log(m_DebugIndex++);
    }
}
