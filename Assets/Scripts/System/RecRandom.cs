using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecRandom
{
    private int m_Seed;
    private static RecRandom m_Instance;
    public static RecRandom Instance
    {   get
        {
            if (m_Instance == null)
                m_Instance = new RecRandom(0);
            return m_Instance;
        }
    }

    public RecRandom(int seed = 0)
    {
        m_Seed = 0;
    }

    public int GetRandomValue(int low, int high)
    {
        return UnityEngine.Random.Range(low, high);
    }

    public float GetRandomValue(float low, float high)
    {
        return UnityEngine.Random.Range(low, high);
    }
}
