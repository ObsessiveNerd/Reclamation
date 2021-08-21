using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static Stack<EscapeableMono> UIMonoBehaviors = new Stack<EscapeableMono>();
    private static bool m_UIClear = true;

    public static bool UIClear => m_UIClear;
    bool m_ClearUINextFrame = false;

    public static void Push(EscapeableMono mono)
    {
        if (!UIMonoBehaviors.Contains(mono))
            UIMonoBehaviors.Push(mono);
    }

    public static void ForcePop(EscapeableMono mono)
    {
        if (UIMonoBehaviors.Peek() == mono)
            UIMonoBehaviors.Pop().OnEscape();
    }

    public static void ForcePop()
    {
        UIMonoBehaviors.Pop().OnEscape();
    }

    public static EscapeableMono GetTopStack()
    {
        return UIMonoBehaviors.Peek();
    }

    public static void RemovePopUntilAllOfTypeRemoved<T>()
    {
        if(UIMonoBehaviors.Count == 0)
            return;

        while (UIMonoBehaviors.Peek().GetType() == typeof(T))
            ForcePop();
    }

    void Update()
    {
        if (m_ClearUINextFrame)
            m_UIClear = true;

        if (UIMonoBehaviors.Count > 0)
            m_UIClear = false;

        if (Input.GetKeyDown(KeyCode.Escape) && !UIClear)
        {
            var escapeMono = UIMonoBehaviors.Pop();
            escapeMono.OnEscape();
        }

        if (UIMonoBehaviors.Count == 0)
            m_ClearUINextFrame = true;
    }
}
