using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    List<T> m_List = new List<T>();
    IComparer<T> m_Comparer;

    public List<T> Values { get { return m_List; } }

    public PriorityQueue(IComparer<T> comparer)
    {
        m_Comparer = comparer;
    }

    public void Add(T obj)
    {
        m_List.Add(obj);
        Sort();
    }

    public void AddRange(IList<T> objs)
    {
        m_List.AddRange(objs);
        Sort();
    }

    public void Remove(T obj)
    {
        m_List.Remove(obj);
    }

    void Sort()
    {
        m_List.Sort(m_Comparer);
    }
}