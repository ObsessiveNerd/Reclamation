using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellExaminationUI : EscapeableMono//, IUpdatableUI
{
    private GameObject Source;
    private Transform Content;
    private TextMeshProUGUI Title;
    private TextMeshProUGUI Body;

    public void Setup(List<string> spellList)
    {
        Source = Instantiate(Resources.Load<GameObject>("Prefabs/SpellExamination"));
        Source.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        Content = Source.transform.Find("Page1");
        Title = Source.transform.Find("Page2Title").GetComponent<TextMeshProUGUI>();
        Body = Source.transform.Find("Page2Body").GetComponent<TextMeshProUGUI>();

        int index = 1;
        foreach (string spellId in spellList)
        {
            IEntity spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameters.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameters.RenderSprite);
            getSpriteEvent.Release();
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
                    //source.FireEvent(GameEventPool.Get(GameEventId.SpellSelected, new .With(EventParameters.Spell, spellId)));
                    //Close();
                });
                index++;
            }
        }


        //WorldUtility.RegisterUI(null);
        //Source.SetActive(true);
        UpdateUI(EntityQuery.GetEntity(spellList[0]));
        UIManager.Push(this);
    }

    public void UpdateUI(IEntity newSource)
    {
        Title.text = newSource.Name;

        Dictionary<string, string> classToInfoMap = new Dictionary<string, string>();
        GameEvent getInfo = GameEventPool.Get(GameEventId.GetInfo)
                                .With(EventParameters.Info, classToInfoMap);

        var result = newSource.FireEvent(getInfo).GetValue<Dictionary<string, string>>(EventParameters.Info);
        StringBuilder sb = new StringBuilder();
        foreach (var s in result.Values)
            sb.AppendLine(s);
        
        getInfo.Release();
        Body.text = sb.ToString();
    }

    public override void OnEscape()
    {
        foreach (Transform go in Content.GetComponentsInChildren<Transform>())
            if (Content != go)
                Destroy(go.gameObject);
        //Source.SetActive(false);
        Destroy(Source);
    }
}
