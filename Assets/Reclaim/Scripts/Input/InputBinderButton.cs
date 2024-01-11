using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputBinderButton : MonoBehaviour
{
    public RequestedAction RequestedAction;
    TextMeshProUGUI m_Text;

    private void Start()
    {
        m_Text = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Button>().onClick.AddListener(() => SetToMyKeyCode());
    }

    public void SetToMyKeyCode()
    {
        GameKeyInputBinder.SetKeyBinder(RequestedAction);
    }

    void Update()
    {
        m_Text.text = $"{RequestedAction}: {GameKeyInputBinder.GetKeyCodeForAction(RequestedAction)}";
    }
}
