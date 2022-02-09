using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagerMono : EscapeableMono
{
    public GameObject CharacterManagerPrefab;
    public Transform CharacterManagerParent;
    public Transform InventoriesView;

    private GameObject m_ViewPrefabInstance;

    public void Setup()
    {
        m_ViewPrefabInstance = Instantiate(CharacterManagerPrefab, transform);
        UIUtility.CreatePlayerInventories(InventoriesView);
        OpenedThisFrame();
    }

    public override bool? AlternativeEscapeKeyPressed
    {
        get
        {
            return Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.OpenInventory)) && !m_OpenedThisFrame;
        }
    }

    public override void OnEscape()
    {
        UIUtility.ClosePlayerInventory();
        Destroy(m_ViewPrefabInstance);
        gameObject.SetActive(false);
    }
}
