using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static Stack<IEscapeableMono> UIMonoBehaviors = new Stack<IEscapeableMono>();
    public static bool UIClear => UIMonoBehaviors.Count == 0;

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
            {
                var contextMenu = ContextMenuMono.CreateNewContextMenu().GetComponent<ContextMenuMono>();
                if(Services.Ready)
                {
                    contextMenu.AddButton(new ContextMenuButton("Return to Main Menu", () =>
                    {
                        Services.SaveAndLoadService.Save();
                        SceneManager.LoadSceneAsync("Reclaim/Scenes/Title");
                    }), null);

                    contextMenu.AddButton(new ContextMenuButton("Save & Exit", () =>
                    {
                        Services.SaveAndLoadService.Save();
                        Application.Quit();
                    }), null);
                }
                contextMenu.AddButton(new ContextMenuButton("Controls", () =>
                    {
                        FindObjectOfType<InputBinder>().Open();
                    }), null);

                contextMenu.AddButton(new ContextMenuButton("Sound", () =>
                    {
                        FindObjectOfType<SoundSettings>().Open();

                    }), null);

                 contextMenu.AddButton(new ContextMenuButton("Display", () =>
                    {
                        FindObjectOfType<DisplaySettings>().Open();
                    }), null);
            }
            else
            {
                var escapeMono = UIMonoBehaviors.Pop();
                escapeMono?.OnEscape();
            }
            m_EscapePressedThisFrame = true;
        }
    }

}
