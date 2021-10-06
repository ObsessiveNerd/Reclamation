using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTab : MonoBehaviour
{
    public Image m_Portrait;
    public TextMeshProUGUI m_PrettyName;
    public TextMeshProUGUI m_Level;
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public Slider m_ExpBar;
    public GameObject m_ActivePlayerIcon;

    private IEntity m_Entity;

    public void Setup(IEntity entity)
    {
        m_Entity = entity;
        m_PrettyName = GetComponentInChildren<TextMeshProUGUI>();

        EventBuilder characterInfo = new EventBuilder(GameEventId.GetPortrait)
                            .With(EventParameters.RenderSprite, null);

        var firedEvent = entity.FireEvent(characterInfo.CreateEvent());

        m_Portrait.sprite = firedEvent.GetValue<Sprite>(EventParameters.RenderSprite);
    }

    public void Update()
    {
        return;
        EventBuilder getHealth = new EventBuilder(GameEventId.GetHealth)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getHealthResult = m_Entity.FireEvent(getHealth.CreateEvent());

        m_HealthBar.maxValue = getHealthResult.GetValue<int>(EventParameters.MaxValue);
        m_HealthBar.value = getHealthResult.GetValue<int>(EventParameters.Value);

        EventBuilder getMana = new EventBuilder(GameEventId.GetMana)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getManaResult = m_Entity.FireEvent(getMana.CreateEvent());

        m_ManaBar.maxValue = getManaResult.GetValue<int>(EventParameters.MaxValue);
        m_ManaBar.value = getManaResult.GetValue<int>(EventParameters.Value);

        EventBuilder getInfo = new EventBuilder(GameEventId.GetName)
                                .With(EventParameters.Name, "");
        m_PrettyName.text = m_Entity.FireEvent(getInfo.CreateEvent()).GetValue<string>(EventParameters.Name);

         EventBuilder getExp = new EventBuilder(GameEventId.GetExperience)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getExpResult = m_Entity.FireEvent(getExp.CreateEvent());

        m_ExpBar.maxValue = getExpResult.GetValue<int>(EventParameters.MaxValue);
        m_ExpBar.value = getExpResult.GetValue<int>(EventParameters.Value);

        EventBuilder getLevel = new EventBuilder(GameEventId.GetLevel)
                                    .With(EventParameters.Level, 0);

        var getLevelResult = m_Entity.FireEvent(getLevel.CreateEvent());
        m_Level.text = getLevelResult.GetValue<int>(EventParameters.Level).ToString();

        if (WorldUtility.IsActivePlayer(m_Entity.ID))
            m_ActivePlayerIcon.SetActive(true);
        else
            m_ActivePlayerIcon.SetActive(false);
    }
}
