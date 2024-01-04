using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CombatLogMono : MonoBehaviour
{
    //public GameObject LHSAnchor, RHSAnchor;
    public GameObject CombatLog;
    public TextMeshProUGUI CombatLogContent;
    //public int MaxLineCount;

    //public GameObject TMPPRefab;
    public GameObject ContentObject;
    RectTransform Content
    {
        get
        {
            return GameObject.Find("CombatLogContent").GetComponent<RectTransform>();
        }
    }

    private List<string> m_EventLog = new List<string>();
    private List<string> m_BacklogEvents = new List<string>();

    private bool m_ProcessingEvent = false;
    //private bool m_IsAtRhs = true;

    void Start()
    {
        RecLog.MessageLogged -= LogMessage;
        RecLog.MessageLogged += LogMessage;
        //MoveCombatLogTo(RHSAnchor.transform);
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
        ResetLogToBottom();
    }

    void RefreshLog()
    {
        //while (m_EventLog.Count > MaxLineCount)
        //    m_EventLog.RemoveAt(0);

        string guiString = string.Empty;
        foreach (string log in m_EventLog)
        {
            //GameObject newLog = new GameObject("CombatLog");
            //var tmp = newLog.AddComponent<TextMeshProUGUI>();
            //tmp.fontSize = 15;
            //tmp.text = log;

            guiString += log;
            guiString += "\n";
        }

        CombatLogContent.text = guiString;
    }

    private void Update()
    {
        RestrictLogScroll();

        //if (false && Input.GetKeyDown(KeyCode.F1))
        //{
        //    if (CombatLog.activeSelf)
        //    {
        //        if (m_IsAtRhs)
        //        {
        //            MoveCombatLogTo(LHSAnchor.transform);
        //            m_IsAtRhs = false;
        //        }
        //        else
        //            CombatLog.SetActive(false);
        //    }
        //    else
        //    {
        //        m_IsAtRhs = true;
        //        MoveCombatLogTo(RHSAnchor.transform);
        //        CombatLog.SetActive(true);
        //    }
        //}
    }

    void RestrictLogScroll()
    {
        var localP = Content.transform.localPosition;
        if(localP.y > 0)
        {
            localP.y = 0;
            Content.transform.localPosition = localP;
        }
    }

    void ResetLogToBottom()
    {
        var localP = Content.transform.localPosition;
        localP.y = 0;
        Content.transform.localPosition = localP;
    }

    //void MoveCombatLogTo(Transform anchor)
    //{
    //    CombatLog.transform.position = anchor.position;
    //}
}
