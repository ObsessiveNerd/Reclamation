using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static Stack<IEscapeableMono> UIMonoBehaviors = new Stack<IEscapeableMono>();
    public static bool UIClear => UIMonoBehaviors.Count == 0;
    static bool m_EscapePressedThisFrame = false;

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
        if (m_EscapePressedThisFrame)
            return;

        if (EscapeForActiveUIPressed.HasValue && EscapeForActiveUIPressed.Value /*&& !UIClear*/)
        {
            if (UIMonoBehaviors.Count == 0)
            {
                var contextMenu = ContextMenuMono.CreateNewContextMenu().GetComponent<ContextMenuMono>();
                if(Services.Ready)
                {
                    string returnToMenuText = Services.StateManagerService.GameEnded ? "Return to Main Menu" : "Save and Return to Menu";
                    contextMenu.AddButton(new ContextMenuButton(returnToMenuText, () =>
                    {
                        if(!Services.StateManagerService.GameEnded)
                            Services.SaveAndLoadService.Save();

                        Services.StateManagerService.CleanGameObjects();
                        SceneManager.LoadSceneAsync("Reclaim/Scenes/Title");

                    }), null);

                    if(!Services.StateManagerService.GameEnded)
                    {
                        contextMenu.AddButton(new ContextMenuButton("Save & Exit", () =>
                        {
                            Services.SaveAndLoadService.Save();
                            Services.StateManagerService.CleanGameObjects();
                            Application.Quit();
                        }), null);
                    }
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

    private void LateUpdate()
    {
        m_EscapePressedThisFrame = false;
    }
}
