using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTab : UpdatableUI
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
    public bool TrackActivePlayer = false;

    public GameObject LevelUp;
    public Button LevelUpButton;

    private IEntity m_Entity;
    private string m_EId;

    public void Setup(IEntity entity)
    {
        m_Entity = entity;
        m_EId = m_Entity.ID;

        GameEvent characterInfo = GameEventPool.Get(GameEventId.GetPortrait)
                            .With(EventParameter.RenderSprite, null);

        var firedEvent = entity.FireEvent(characterInfo);

        m_Portrait.sprite = firedEvent.GetValue<Sprite>(EventParameter.RenderSprite);
        firedEvent.Release();
    }

    public override void UpdateUI()
    {
        if (TrackActivePlayer)
            m_Entity = Services.PlayerManagerService.GetActivePlayer();

        if (m_Entity == null)
            m_Entity = EntityQuery.GetEntity(m_EId);

        GameEvent getHealth = GameEventPool.Get(GameEventId.GetHealth)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);

        var getHealthResult = m_Entity.FireEvent(getHealth);

        m_HealthBar.maxValue = getHealthResult.GetValue<int>(EventParameter.MaxValue);
        m_HealthBar.value = getHealthResult.GetValue<int>(EventParameter.Value);
        HealthText.text = $"{m_HealthBar.value}/{m_HealthBar.maxValue}";

        getHealthResult.Release();

        var getAttribute = GameEventPool.Get(GameEventId.GetAttributePoints)
                                            .With(EventParameter.AttributePoints, 0);
        m_Entity.FireEvent(getAttribute);

        if (getAttribute.GetValue<int>(EventParameter.AttributePoints) > 0)
            LevelUp.SetActive(true);
        else
            LevelUp.SetActive(false);

        getAttribute.Release();

        GameEvent getMana = GameEventPool.Get(GameEventId.GetMana)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);

        var getManaResult = m_Entity.FireEvent(getMana);

        m_ManaBar.maxValue = getManaResult.GetValue<int>(EventParameter.MaxValue);
        m_ManaBar.value = getManaResult.GetValue<int>(EventParameter.Value);
        ManaText.text = $"{m_ManaBar.value}/{m_ManaBar.maxValue}";
        getManaResult.Release();

        GameEvent getInfo = GameEventPool.Get(GameEventId.GetName)
                                .With(EventParameter.Name, "");
        m_PrettyName.text = m_Entity.FireEvent(getInfo).GetValue<string>(EventParameter.Name);
        getInfo.Release();

         GameEvent getExp = GameEventPool.Get(GameEventId.GetExperience)
                                    .With(EventParameter.Value, 0)
                                    .With(EventParameter.MaxValue, 0);

        var getExpResult = m_Entity.FireEvent(getExp);

        m_ExpBar.maxValue = getExpResult.GetValue<int>(EventParameter.MaxValue);
        m_ExpBar.value = getExpResult.GetValue<int>(EventParameter.Value);
        ExpText.text = $"{m_ExpBar.value}/{m_ExpBar.maxValue}";
        getExpResult.Release();

        GameEvent getLevel = GameEventPool.Get(GameEventId.GetLevel)
                                    .With(EventParameter.Level, 0);

        var getLevelResult = m_Entity.FireEvent(getLevel);
        m_Level.text = getLevelResult.GetValue<int>(EventParameter.Level).ToString();
        getLevelResult.Release();

        LevelUpButton.onClick.RemoveAllListeners();
        LevelUpButton.onClick.AddListener(() =>
        {
            Services.PlayerManagerService.SetActiveCharacter(m_EId);
            Services.WorldUIService.OpenInventory();
            Services.WorldUIService.SetCharacterView("Stats");
        });

        if (WorldUtility.IsActivePlayer(m_Entity.ID) && ShowActivePlayerIcon)
            m_ActivePlayerIcon.SetActive(true);
        else
            m_ActivePlayerIcon.SetActive(false);
    }
}
