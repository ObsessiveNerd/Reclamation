using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagerMono : EscapeableMono, IUpdatableUI
{
    private bool m_OpenedThisFrame = false;
    public GameObject CharacterManagerObject;
    public Transform InventoriesView;

    private Dictionary<IEntity, CharacterMono> characters = new Dictionary<IEntity, CharacterMono>();
    private GameObject m_ViewPrefab;
    private GameObject m_ViewPrefabInstance;

    List<GameObject> m_Inventories;
    CharacterMono m_CharacterMono;

    public void Setup(IEntity source)
    {
        UIManager.Push(this);
        WorldUtility.RegisterUI(this);
        CharacterManagerObject.SetActive(true);
        m_OpenedThisFrame = true;
        m_ViewPrefab = Resources.Load<GameObject>("Prefabs/CharacterManager");

            m_Inventories = UIUtility.CreatePlayerInventories(InventoriesView);

        m_ViewPrefabInstance = Instantiate(m_ViewPrefab, CharacterManagerObject.transform);
        //go.transform.SetParent(CharacterManagerObject.transform);
        m_CharacterMono = m_ViewPrefabInstance.GetComponent<CharacterMono>();
        UpdateUI(source);
    }

    public void AddCharacter(IEntity source)
    {
        return;
        if (characters.ContainsKey(source))
            return;

        m_ViewPrefabInstance = Instantiate(m_ViewPrefab, CharacterManagerObject.transform);
        //go.transform.SetParent(CharacterManagerObject.transform);
        m_CharacterMono = m_ViewPrefabInstance.GetComponent<CharacterMono>();
        m_CharacterMono.Setup(source);
        //characters.Add(source, cm);
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
        WorldUtility.UnRegisterUI(this);
        UIManager.ForcePop(this);
        UIUtility.ClosePlayerInventory();
        CharacterManagerObject.SetActive(false);
        Destroy(m_ViewPrefabInstance);
        
    }

    public void UpdateUI(IEntity newSource)
    {
        IEntity activePlayer = Services.EntityMapService.GetEntity(Services.WorldDataQuery.GetActivePlayerId());
        m_CharacterMono.Setup(activePlayer);

        //foreach(var inventory in m_Inventories)
        //    inventory.GetComponent<InventoryManagerMono>().Setup(null);

        //if(characters.ContainsKey(newSource))
        //    characters[newSource].Setup(newSource);
    }
}
