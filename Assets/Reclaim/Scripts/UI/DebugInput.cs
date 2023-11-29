using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugInput : EscapeableMono
{
    TMP_InputField m_InputField;

    protected override void OnEnable()
    {
        Services.StateManagerService.TimeProgress.Stop();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        Services.StateManagerService.TimeProgress.Resume();
        base.OnDisable();
    }

    public override void OnEscape()
    {
        m_InputField.text = "";
        gameObject.SetActive(false);
    }

    void Start()
    {
        m_InputField = GetComponent<TMP_InputField>();
        m_InputField.Select();
        m_InputField.ActivateInputField();
        m_InputField.onEndEdit.AddListener((str) =>
        {
            IEntity e = EntityFactory.CreateEntity(str);
            if (e == null)
            {
                m_InputField.text = "";
                m_InputField.Select();
                m_InputField.ActivateInputField();
                return;
            }

            GameEvent giveItem = GameEventPool.Get(GameEventId.AddToInventory)
                                    .With(EventParameter.Entity, e.ID);

            Services.PlayerManagerService.GetActivePlayer().FireEvent(giveItem);
            giveItem.Release();

            Services.WorldUIService.UpdateUI();

            m_InputField.text = "";
            m_InputField.Select();
            m_InputField.ActivateInputField();
        });
    }
}
