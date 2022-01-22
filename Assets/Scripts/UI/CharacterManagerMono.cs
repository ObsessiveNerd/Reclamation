using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagerMono : EscapeableMono, IUpdatableUI
{
    private bool m_OpenedThisFrame = false;
    public GameObject CharacterManagerObject;

    private Dictionary<IEntity, CharacterMono> characters = new Dictionary<IEntity, CharacterMono>();
    private GameObject m_ViewPrefab;
    public void Setup(IEntity source)
    {
        UIManager.Push(this);
        WorldUtility.RegisterUI(this);
        CharacterManagerObject.SetActive(true);
        m_OpenedThisFrame = true;
        m_ViewPrefab = Resources.Load<GameObject>("Prefabs/CharacterManager");
    }

    public void AddCharacter(IEntity source)
    {
        if (characters.ContainsKey(source))
            return;

        GameObject go = Instantiate(m_ViewPrefab, CharacterManagerObject.transform);
        //go.transform.SetParent(CharacterManagerObject.transform);
        CharacterMono cm = go.GetComponent<CharacterMono>();
        cm.Setup(source);
        characters.Add(source, cm);
    }

    public override bool? AlternativeEscapeKeyPressed
    {
        get
        {
            return Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.OpenInventory)) && !m_OpenedThisFrame;
        }
    }

    public void LateUpdate()
    {
        m_OpenedThisFrame = false;
    }

    public override void OnEscape()
    {
        Cleanup();
        CharacterManagerObject.SetActive(false);
    }

    void Cleanup()
    {
        foreach(var cm in characters.Keys)
        {
            characters[cm].Cleanup();
            Destroy(characters[cm].gameObject);
        }
        characters.Clear();
        WorldUtility.UnRegisterUI(this);
    }

    public void UpdateUI(IEntity newSource)
    {
        characters[newSource].Setup(newSource);
    }
}
