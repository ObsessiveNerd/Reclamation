using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static bool ExistsAlready = false;
    static Stack<IEscapeableMono> UIMonoBehaviors = new Stack<IEscapeableMono>();
    public static bool UIClear => UIMonoBehaviors.Count == 0;

    private void Start()
    {
        if (ExistsAlready)
        {
            Destroy(gameObject);
            return;
        }

        ExistsAlready = true;
    }

    public static void Push(IEscapeableMono mono)
    {
        if (mono == null)
            Debug.LogWarning("pushing null to the UI manager stack.  Could cause issues");
        else
            Debug.LogWarning($"Pusing {mono.GetType().Name} to UI stack");

        if (mono == null || !UIMonoBehaviors.Contains(mono))
            UIMonoBehaviors.Push(mono);
    }

    public static void ForcePop(IEscapeableMono mono)
    {
        if (UIMonoBehaviors.Count > 0 && UIMonoBehaviors.Peek() == mono)
        {
            Debug.LogWarning($"{mono.GetType().Name} popping from UI stack");
            UIMonoBehaviors.Pop().OnEscape();
        }
    }

    public static void ForcePop()
    {
        if (UIMonoBehaviors.Count > 0)
        {
            var behavior = UIMonoBehaviors.Pop();
            Debug.LogWarning($"{behavior} is being popped from UI stack.");
            behavior?.OnEscape();
        }
    }

    public static IEscapeableMono GetTopStack()
    {
        return UIMonoBehaviors.Peek();
    }

    public static void RemovePopUntilAllOfTypeRemoved<T>()
    {
        if (UIMonoBehaviors.Count == 0)
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

            if (UIMonoBehaviors.Count == 0)
                return false;

            return UIMonoBehaviors.Peek()?.AlternativeEscapeKeyPressed.Value;
        }
    }

    bool m_EscapePressedThisFrame = false;
    void Update()
    {
        if (m_EscapePressedThisFrame)
            m_EscapePressedThisFrame = false;
        else if (EscapeForActiveUIPressed.HasValue && EscapeForActiveUIPressed.Value /*&& !UIClear*/)
        {
            if (UIMonoBehaviors.Count == 0)
                FindObjectOfType<InputBinder>().Open();
            else
            {
                var escapeMono = UIMonoBehaviors.Pop();
                escapeMono?.OnEscape();
            }
            m_EscapePressedThisFrame = true;
        }
    }

}
