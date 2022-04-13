using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugInput : MonoBehaviour
{
    TMP_InputField m_InputField;

    void Start()
    {
        m_InputField = GetComponent<TMP_InputField>();
        m_InputField.onEndEdit.AddListener((str) =>
        {
            GameEvent giveItem = GameEventPool.Get(GameEventId.AddToInventory)
                                    .With(EventParameters.Entity, EntityFactory.CreateEntity(str).ID);

            Services.PlayerManagerService.GetActivePlayer().FireEvent(giveItem);
            giveItem.Release();

            Services.WorldUIService.UpdateUI();

            m_InputField.text = "";
            gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tilde))
            gameObject.SetActive(false);
    }
}
