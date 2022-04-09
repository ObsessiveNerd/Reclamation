using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecRandom
{
    public int Seed;
    private static RecRandom m_Instance;
    public static RecRandom Instance
    {   get
        {
            if (m_Instance == null)
                m_Instance = new RecRandom();
            return m_Instance;
        }
    }

    public static int InitRecRandom(int seed = 0)
    {
        m_Instance = new RecRandom();
        m_Instance.Seed = seed;
        UnityEngine.Random.InitState(m_Instance.Seed);
        return m_Instance.Seed;
    }

    public int GetRandomValue(int low, int high)
    {
        return UnityEngine.Random.Range(low, high);
    }

    public int GetRandomPercent()
    {
        return GetRandomValue(0, 100);
    }

    public float GetRandomValue(float low, float high)
    {
        return UnityEngine.Random.Range(low, high);
    }
}
