using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuMono : EscapeableMono
{
    public Transform Content;

    public static GameObject CreateNewContextMenu()
    {
        var contextMenu = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ContextMenu"));
        contextMenu.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform, false);
        return contextMenu;
    }

    public void AddButton(ContextMenuButton cmb, IEntity source, Action afterClickCallback = null)
    {
        GameObject instance = cmb.CreateButton(Resources.Load<GameObject>("Prefabs/UI/ContextMenuButton"));
        Button button = instance.GetComponent<Button>();

        button.onClick.AddListener(() => UIManager.ForcePop(this));
        button.onClick.AddListener(() => Services.WorldUIService.UpdateUI());

        if (afterClickCallback != null)
            button.onClick.AddListener(() => afterClickCallback());

        instance.transform.SetParent(Content);
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
        Destroy(gameObject);
    }
}
