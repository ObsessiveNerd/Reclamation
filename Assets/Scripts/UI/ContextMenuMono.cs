using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuMono : EscapeableMono
{
    public GameObject ContextMenuButton;
    public GameObject ContextMenu;

    List<GameObject> m_Buttons = new List<GameObject>();

    public void AddButton(ContextMenuButton cmb)
    {
        ContextMenu.SetActive(true);

        GameObject instance = cmb.CreateButton(ContextMenuButton);
        Button button = instance.GetComponent<Button>();

        button.onClick.AddListener(() => OnEscape());
        button.onClick.AddListener(() =>
        {
            World.Instance.Self.FireEvent(new GameEvent(GameEventId.UpdateUI));
        });

        instance.transform.SetParent(ContextMenu.transform);
        m_Buttons.Add(instance);
    }

    protected override void OnEscape()
    {
        foreach (var button in m_Buttons)
            Destroy(button);
        ContextMenu.SetActive(false);
    }
}
