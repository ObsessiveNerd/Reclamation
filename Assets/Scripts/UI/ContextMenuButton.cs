using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ContextMenuButton
{
    public Action ActionToTake;
    public string Text;

    public ContextMenuButton(string text, Action action)
    {
        ActionToTake = action;
        Text = text;
    }

    public GameObject CreateButton(GameObject prefab)
    {
        Action action = ActionToTake;
        GameObject obj = GameObject.Instantiate(prefab);
        obj.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
        obj.GetComponentInChildren<TextMeshProUGUI>().text = Text;
        return obj;
    }
}
