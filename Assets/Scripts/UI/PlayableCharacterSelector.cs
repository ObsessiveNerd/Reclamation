using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterSelector : MonoBehaviour
{
    public GameObject CharacterButton;

    void Start()
    {
        foreach (Transform go in GetComponentsInChildren<Transform>())
            if (transform != go)
                Destroy(go.gameObject);

        EventBuilder getActivePlayers = new EventBuilder(GameEventId.GetPlayableCharacters)
                                        .With(EventParameters.Value, new List<string>());

        List<string> activePlayerIds = World.Instance.Self.FireEvent(getActivePlayers.CreateEvent()).GetValue<List<string>>(EventParameters.Value);

        foreach(string id in activePlayerIds)
        {
            IEntity entity = EntityQuery.GetEntity(id);
            EventBuilder characterInfo = new EventBuilder(GameEventId.GetCharacterInfo)
                                        .With(EventParameters.Name, "")
                                        .With(EventParameters.RenderSprite, null);

            var firedEvent = entity.FireEvent(characterInfo.CreateEvent());

            var newTab = Instantiate(CharacterButton);
            newTab.transform.SetParent(transform);
            newTab.GetComponent<CharacterTab>().Setup(firedEvent.GetValue<Sprite>(EventParameters.RenderSprite), firedEvent.GetValue<string>(EventParameters.Name));
        }
    }
}
