using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellExaminationUI : EscapeableMono//, IUpdatableUI
{
    public GameObject Source;
    public Transform Content;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Body;

    public void Setup(List<string> spellList)
    {
        int index = 1;
        foreach (string spellId in spellList)
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
                spriteGo.transform.SetParent(Content);

                spriteGo.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();

                Button button = spriteGo.AddComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    UpdateUI(spell);
                    //source.FireEvent(new GameEvent(GameEventId.SpellSelected, new KeyValuePair<string, object>(EventParameters.Spell, spellId)));
                    //Close();
                });
                index++;
            }
        }


        //WorldUtility.RegisterUI(null);
        Source.SetActive(true);
        Source.transform.SetAsLastSibling();
        UpdateUI(EntityQuery.GetEntity(spellList[0]));
        UIManager.Push(this);
    }

    public void UpdateUI(IEntity newSource)
    {
        Title.text = newSource.Name;

        Dictionary<string, string> classToInfoMap = new Dictionary<string, string>();
        EventBuilder getInfo = EventBuilderPool.Get(GameEventId.GetInfo)
                                .With(EventParameters.Info, classToInfoMap);

        var result = newSource.FireEvent(getInfo.CreateEvent()).GetValue<Dictionary<string, string>>(EventParameters.Info);
        StringBuilder sb = new StringBuilder();
        foreach (var s in result.Values)
            sb.AppendLine(s);

        Body.text = sb.ToString();
    }

    public override void OnEscape()
    {
        foreach (Transform go in Content.GetComponentsInChildren<Transform>())
            if (Content != go)
                Destroy(go.gameObject);
        Source.SetActive(false);
    }
}
