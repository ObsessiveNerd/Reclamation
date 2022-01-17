using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuMono : EscapeableMono
{
    public Transform Content;
    //List<GameObject> m_Buttons = new List<GameObject>();

    public static GameObject CreateNewContextMenu()
    {
        var contextMenu = Instantiate(Resources.Load<GameObject>("UI/ContextMenu"));
        contextMenu.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform, false);
        UIManager.Push(contextMenu.GetComponent<ContextMenuMono>());
        return contextMenu;
    }

    public void AddButton(ContextMenuButton cmb, IEntity source, Action afterClickCallback = null)
    {
        GameObject instance = cmb.CreateButton(Resources.Load<GameObject>("UI/ContextMenuButton"));
        Button button = instance.GetComponent<Button>();

        //button.onClick.AddListener(() => OnEscape());
        button.onClick.AddListener(() => UIManager.ForcePop(this));
        button.onClick.AddListener(() =>
        {
            World.Instance.Self.FireEvent(new GameEvent(GameEventId.UpdateUI, new KeyValuePair<string, object>(EventParameters.Entity, source.ID)));
        });
        if (afterClickCallback != null)
            button.onClick.AddListener(() => afterClickCallback());

        instance.transform.SetParent(Content);
        //m_Buttons.Add(instance);
    }

    public void SelectPlayer(Action<string> actionForSelectedPlayer, List<string> playerIds)
    {
        foreach (string id in playerIds)
        {
            ContextMenuButton button = new ContextMenuButton(EntityQuery.GetEntity(id).Name, () =>
            {
                actionForSelectedPlayer(id);
            });
            
            AddButton(button, EntityQuery.GetEntity(id), () => UIManager.ForcePop());
        }
    }

    public override void OnEscape()
    {
        //if(UIManager.GetTopStack() == this)
            Destroy(gameObject);
        //foreach (var button in m_Buttons)
        //    Destroy(button);
        //m_Buttons.Clear();
    }
}
