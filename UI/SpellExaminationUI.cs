using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellExaminationUI : EscapeableMono//, IUpdatableUI
{
    private Transform Content;
    private TextMeshProUGUI Title;
    private TextMeshProUGUI Body;

    GameObject m_ActiveSpell;

    public void Setup(List<string> spellList)
    {
        Content = transform.Find("Page1");
        Title = transform.Find("Page2Title").GetComponent<TextMeshProUGUI>();
        Body = transform.Find("Page2Body").GetComponent<TextMeshProUGUI>();

        int index = 1;
        foreach (string spellId in spellList)
        {
            GameObject spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("Prefabs/UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameter.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameter.RenderSprite);
            getSpriteEvent.Release();
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(Content);
                spriteGo.GetComponent<SpellAbilityUIMono>().Setup(Services.PlayerManagerService.GetActivePlayer(), spell, false, false);

                Button button = spriteGo.AddComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    UpdatePage(spell);
                });
                index++;
            }
        }

        UpdatePage(EntityQuery.GetEntity(spellList[0]));
    }

    public void UpdatePage(GameObject newSource)
    {
        m_ActiveSpell = newSource;
        Title.text = newSource.Name;

        Dictionary<string, string> classToInfoMap = new Dictionary<string, string>();
        GameEvent getInfo = GameEventPool.Get(GameEventId.GetInfo)
                                .With(EventParameter.Info, classToInfoMap);

        var result = newSource.FireEvent(getInfo).GetValue<Dictionary<string, string>>(EventParameter.Info);
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
        gameObject.SetActive(false);
    }
}
