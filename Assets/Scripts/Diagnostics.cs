using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DiagnosticsTimer : IDisposable
{
    Stopwatch m_StopWatch;
    string m_ID;

    public DiagnosticsTimer(string id)
    {
        m_StopWatch = new Stopwatch();
        m_StopWatch.Start();
        m_ID = id;
    }

    public void Dispose()
    {
        m_StopWatch.Stop();
        UnityEngine.Debug.LogWarning($"{m_ID} took {m_StopWatch.Elapsed.Seconds} seconds");
    }
}
