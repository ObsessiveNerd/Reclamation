using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static Stack<EscapeableMono> UIMonoBehaviors = new Stack<EscapeableMono>();
    public static bool UIClear => UIMonoBehaviors.Count == 0;

    public static void Push(EscapeableMono mono)
    {
        if (mono == null)
            Debug.LogWarning("pushing null to the UI manager stack.  Could cause issues");
        else
            Debug.LogWarning($"Pusing {mono.name} to UI stack");

        if (!UIMonoBehaviors.Contains(mono))
            UIMonoBehaviors.Push(mono);
    }

    public static void ForcePop(EscapeableMono mono)
    {
        if (UIMonoBehaviors.Count > 0 && UIMonoBehaviors.Peek() == mono)
        {
            Debug.LogWarning($"{mono.name} popping from UI stack");
            UIMonoBehaviors.Pop().OnEscape();
        }
    }

    public static void ForcePop()
    {
        if(UIMonoBehaviors.Count > 0)
        {
            var behavior = UIMonoBehaviors.Pop();
            Debug.LogWarning($"{behavior} is being popped from UI stack.");
            behavior?.OnEscape();
        }
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

    public static bool? EscapeForActiveUIPressed
    {
        get
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                return true;

            return UIMonoBehaviors.Peek()?.AlternativeEscapeKeyPressed.Value;
        }
    }

    void Update()
    {
        if (UIMonoBehaviors.Count == 0)
            return;

        Debug.Log(UIMonoBehaviors.Peek());

        if (EscapeForActiveUIPressed.HasValue && EscapeForActiveUIPressed.Value /*&& !UIClear*/)
        {
            var escapeMono = UIMonoBehaviors.Pop();
            escapeMono?.OnEscape();
        }
    }
}
