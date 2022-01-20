using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayableCharacterSelector : MonoBehaviour
{
    public GameObject CharacterButton;

    private Dictionary<string, GameObject> m_CharacterIdToTabGameObject = new Dictionary<string, GameObject>();
    public void AddCharacterTab(string id)
    {
        if (m_CharacterIdToTabGameObject.ContainsKey(id))
            return;

        IEntity entity = EntityQuery.GetEntity(id);

        var newTab = Instantiate(CharacterButton);
        newTab.transform.SetParent(transform);
        newTab.GetComponent<CharacterTab>().Setup(entity);

        newTab.AddComponent<Button>().onClick.AddListener(() =>
        {
            GameEvent newPlayer = GameEventPool.Get(GameEventId.SetActiveCharacter)
                                        .With(EventParameters.Entity, id);

            World.Services.Self.FireEvent(newPlayer).Release();
        });

        m_CharacterIdToTabGameObject.Add(id, newTab);
    }

    public void RemoveCharacterTab(string id)
    {
        if(m_CharacterIdToTabGameObject.ContainsKey(id))
        {
            Destroy(m_CharacterIdToTabGameObject[id]);
            m_CharacterIdToTabGameObject.Remove(id);
        }
    }
}
