using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIMono : MonoBehaviour//, IUpdatableUI
{
    public Stat ControlledStat;
    TextMeshProUGUI Text;
    IEntity m_Source;
    private Button m_Button;

    public Button Button
    {
        get
        {
            if (m_Button == null)
            {
                m_Button = GetComponentInChildren<Button>();
            }
            return m_Button;

        }
    }

    public void Setup(IEntity source)
    {
        if(Text == null)
            Text = GetComponent<TextMeshProUGUI>();

        m_Source = source;

        EventBuilder getStat = EventBuilderPool.Get(GameEventId.GetStatRaw)
                                .With(EventParameters.StatType, ControlledStat)
                                .With(EventParameters.Value, Stat.Str);

        int value = source.FireEvent(getStat.CreateEvent()).GetValue<int>(EventParameters.Value);
        Text.text = $"{ControlledStat.ToString()}: {value}";
    }

    public void UpdateUI(IEntity newSource)
    {
        Setup(newSource);
    }
}
