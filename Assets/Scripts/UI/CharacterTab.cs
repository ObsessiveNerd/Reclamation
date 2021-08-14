using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTab : MonoBehaviour
{
    public Image m_Portrait;
    public TextMeshProUGUI m_PrettyName;
    public Slider m_HealthBar;
    public Slider m_ManaBar;

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
        EventBuilder getHealth = new EventBuilder(GameEventId.GetHealth)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getHealthResult = m_Entity.FireEvent(getHealth.CreateEvent());

        m_HealthBar.maxValue = getHealthResult.GetValue<int>(EventParameters.MaxValue);
        m_HealthBar.value = getHealthResult.GetValue<int>(EventParameters.Value);
    }
}
