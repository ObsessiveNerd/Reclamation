using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsUIMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject m_Popup;
    private GameObject m_PopupInstance;

    public Stat ControlledStat;
    public TextMeshProUGUI Text;
    public Button Button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_PopupInstance = Instantiate(m_Popup);
        m_PopupInstance.GetComponent<InfoMono>().SetData(GetFullStatName(), GetInfoForControlledStat());
        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(m_PopupInstance);
    }

    protected void OnDisable()
    {
        Destroy(m_PopupInstance);
        m_PopupInstance = null;
    }

    public void SetData()
    {
        IEntity source = Services.PlayerManagerService.GetActivePlayer();
        
        if (Text == null)
            Text = GetComponent<TextMeshProUGUI>();

        m_Popup = Resources.Load<GameObject>("Prefabs/UI/ItemPopup");
        GameEvent getStat = GameEventPool.Get(GameEventId.GetStatRaw)
                                .With(EventParameters.StatType, ControlledStat)
                                .With(EventParameters.Value, Stat.Str);

        int value = source.FireEvent(getStat).GetValue<int>(EventParameters.Value);
        Text.text = $"{ControlledStat.ToString()}: {value}";
        getStat.Release();
    }

    string GetFullStatName()
    {
        string result = "";
        switch (ControlledStat)
        {
            case Stat.Str:
                result = "Strength";
                break;
            case Stat.Agi:
                result = "Agility";
                break;
            case Stat.Con:
                result = "Constitution";
                break;
            case Stat.Wis:
                result = "Wisdom";
                break;
            case Stat.Int:
                result = "Intelligence";
                break;
            case Stat.Cha:
                result = "Charisma";
                break;
        }
        return result;
    }

    string GetInfoForControlledStat()
    {
        string result = "";
        switch (ControlledStat)
        {
            case Stat.Str:
                result = "Affects how often, and how hard, you hit with melee attacks.";
                break;
            case Stat.Agi:
                result = "Affects how accurate you are with ranged physical weapons, or melee weapons with the Finesse attribute.";
                break;
            case Stat.Con:
                result = "Affects your max Health.";
                break;
            case Stat.Wis:
                //result = "Ugh.. I guess this only contributes to saving throws right now.";
                break;
            case Stat.Int:
                result = "Affects your maximum Mana.";
                break;
            case Stat.Cha:
                //result = "Ugh.. I guess this only contributes to saving throws right now.";
                break;
        }

        result += "\nIncreases the difficulty of a saving throw your target must make for spells cast and abilities used that benefit from this stat.";
        return result;
    }
}
