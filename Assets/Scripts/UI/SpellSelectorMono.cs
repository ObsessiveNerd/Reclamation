using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectorMono : EscapeableMono
{
    public GameObject SpellObject;
    public GameObject SpellView;

    public void Setup(IEntity source, IEnumerable<string> spellList)
    {
        foreach(string spellId in spellList)
        {
            IEntity spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("UI/SpellUI");
            Sprite sprite = spell.FireEvent(spell, new GameEvent(GameEventId.GetSprite, 
                new KeyValuePair<string, object>(EventParameters.RenderSprite, null))).GetValue<Sprite>(EventParameters.RenderSprite);
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(SpellView.transform);

                Button button = spriteGo.AddComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    source.FireEvent(new GameEvent(GameEventId.SpellSelected, new KeyValuePair<string, object>(EventParameters.Spell, spellId)));
                    Close();
                });
            }
        }

        SpellObject.SetActive(true);
        UIManager.Push(this);
    }

    public override void OnEscape()
    {
        Close();
    }

    void Close()
    {
        SpellObject.SetActive(false);
        foreach (Transform go in SpellView.GetComponentsInChildren<Transform>())
            if(SpellView.transform != go)
                Destroy(go.gameObject);
    }
}
