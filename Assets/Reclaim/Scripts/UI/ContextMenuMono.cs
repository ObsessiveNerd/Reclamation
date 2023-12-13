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
        contextMenu.transform.SetParent(GameObject.Find("Canvas").transform, false);
        return contextMenu;
    }

    public void AddButton(ContextMenuButton cmb, GameObject source, Action afterClickCallback = null)
    {
        GameObject instance = cmb.CreateButton(Resources.Load<GameObject>("Prefabs/UI/ContextMenuButton"));
        Button button = instance.GetComponent<Button>();

        button.onClick.AddListener(() => UIManager.ForcePop(this));
        button.onClick.AddListener(() =>
        {
            if (Services.Ready)
                Services.WorldUIService.UpdateUI();
        });

        if (afterClickCallback != null)
            button.onClick.AddListener(() => afterClickCallback());

        instance.transform.SetParent(Content, false);
    }

    public void GetAmountToGive(Action<GameObject, GameObject, int> actionForSelectedPlayer, GameObject source, GameObject item)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TextInputField"));
        instance.GetComponent<TextFieldMono>().Init("Amount to give...");
        instance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        instance.SetActive(true);

        instance.GetComponent<TMP_InputField>().onEndEdit.AddListener((arg) =>
        {
            if(int.TryParse(arg, out int result))
            {
                actionForSelectedPlayer.Invoke(source, item, result);
            }
            else
            {
                actionForSelectedPlayer.Invoke(source, item, 1);
            }

            Destroy(instance);
        });
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
