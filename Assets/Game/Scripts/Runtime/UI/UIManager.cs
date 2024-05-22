using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static Stack<IEscapeableMono> UIMonoBehaviors = new Stack<IEscapeableMono>();
    public static bool UIClear => UIMonoBehaviors.Count == 0;
    static bool m_EscapePressedThisFrame = false;

    public GameObject Inventory;
    public GameObject Equipment;

    private void Start()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;    
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;    
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        try
        {
            while (UIMonoBehaviors.Count > 0)
                UIMonoBehaviors.Pop()?.OnEscape();
        }
        catch
        {
            //do nothing
        }
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
            m_EscapePressedThisFrame = true;
        }
    }

    public static void ForcePop()
    {
        if (UIMonoBehaviors.Count > 0)
        {
            var behavior = UIMonoBehaviors.Pop();
            Debug.LogWarning($"{behavior} is being popped from UI stack.");
            behavior?.OnEscape();
            m_EscapePressedThisFrame = true;
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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Inventory.SetActive(!Inventory.activeInHierarchy);
            Equipment.SetActive(!Equipment.activeInHierarchy);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Inventory.SetActive(false);
            Equipment.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        m_EscapePressedThisFrame = false;
    }
}
