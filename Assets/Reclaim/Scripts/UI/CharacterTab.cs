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

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ManaText;
    public TextMeshProUGUI ExpText;
    
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public Slider m_ExpBar;
    public GameObject m_ActivePlayerIcon;
    public bool ShowActivePlayerIcon = true;

    private IEntity m_Entity;
    private string m_EId;

    public void Setup(IEntity entity)
    {
        m_Entity = entity;
        m_EId = m_Entity.ID;

        GameEvent characterInfo = GameEventPool.Get(GameEventId.GetPortrait)
                            .With(EventParameters.RenderSprite, null);

        var firedEvent = entity.FireEvent(characterInfo);

        m_Portrait.sprite = firedEvent.GetValue<Sprite>(EventParameters.RenderSprite);
        firedEvent.Release();
    }

    public void Update()
    {
        if (m_Entity == null)
            m_Entity = EntityQuery.GetEntity(m_EId);

        GameEvent getHealth = GameEventPool.Get(GameEventId.GetHealth)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getHealthResult = m_Entity.FireEvent(getHealth);

        m_HealthBar.maxValue = getHealthResult.GetValue<int>(EventParameters.MaxValue);
        m_HealthBar.value = getHealthResult.GetValue<int>(EventParameters.Value);
        HealthText.text = $"{m_HealthBar.value}/{m_HealthBar.maxValue}";

        getHealthResult.Release();

        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getManaResult = m_Entity.FireEvent(getMana);

        m_ManaBar.maxValue = getManaResult.GetValue<int>(EventParameters.MaxValue);
        m_ManaBar.value = getManaResult.GetValue<int>(EventParameters.Value);
        ManaText.text = $"{m_ManaBar.value}/{m_ManaBar.maxValue}";
        getManaResult.Release();

        GameEvent getInfo = GameEventPool.Get(GameEventId.GetName)
                                .With(EventParameters.Name, "");
        m_PrettyName.text = m_Entity.FireEvent(getInfo).GetValue<string>(EventParameters.Name);
        getInfo.Release();

         GameEvent getExp = GameEventPool.Get(GameEventId.GetExperience)
                                    .With(EventParameters.Value, 0)
                                    .With(EventParameters.MaxValue, 0);

        var getExpResult = m_Entity.FireEvent(getExp);

        m_ExpBar.maxValue = getExpResult.GetValue<int>(EventParameters.MaxValue);
        m_ExpBar.value = getExpResult.GetValue<int>(EventParameters.Value);
        ExpText.text = $"{m_ExpBar.value}/{m_ExpBar.maxValue}";
        getExpResult.Release();

        GameEvent getLevel = GameEventPool.Get(GameEventId.GetLevel)
                                    .With(EventParameters.Level, 0);

        var getLevelResult = m_Entity.FireEvent(getLevel);
        m_Level.text = getLevelResult.GetValue<int>(EventParameters.Level).ToString();
        getLevelResult.Release();

        if (WorldUtility.IsActivePlayer(m_Entity.ID) && ShowActivePlayerIcon)
            m_ActivePlayerIcon.SetActive(true);
        else
            m_ActivePlayerIcon.SetActive(false);
    }
}
