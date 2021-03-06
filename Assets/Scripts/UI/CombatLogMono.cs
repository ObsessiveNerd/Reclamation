using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CombatLogMono : MonoBehaviour
{
    public GameObject CombatLog;
    public TextMeshProUGUI CombatLogContent;
    public int MaxLineCount;

    private List<string> m_EventLog = new List<string>();
    private List<string> m_BacklogEvents = new List<string>();

    private bool m_ProcessingEvent = false;

    void Start()
    {
        RecLog.MessageLogged += LogMessage;
    }

    void LogMessage(string message)
    {
        if (!m_ProcessingEvent)
        {
            m_ProcessingEvent = true;
            m_EventLog.Add(message);
            RefreshLog();
            m_ProcessingEvent = false;
            if(m_BacklogEvents.Count > 0)
            {
                string s = m_BacklogEvents[0];
                m_BacklogEvents.RemoveAt(0);
                LogMessage(s);
            }
        }
        else
        {
            m_BacklogEvents.Add(message);
        }
    }

    void RefreshLog()
    {
        while (m_EventLog.Count > MaxLineCount)
            m_EventLog.RemoveAt(0);

        string guiString = string.Empty;
        foreach (string log in m_EventLog)
        {
            guiString += log;
            guiString += "\n";
        }

        CombatLogContent.text = guiString;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            CombatLog.SetActive(!CombatLog.activeSelf);
    }
}
