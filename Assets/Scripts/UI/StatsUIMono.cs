using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIMono : MonoBehaviour//, IUpdatableUI
{
    public Stat ControlledStat;
    public TextMeshProUGUI Text;
    public Button Button;

    IEntity m_Source;

    public void Setup(IEntity source)
    {
        if(Text == null)
            Text = GetComponent<TextMeshProUGUI>();

        m_Source = source;

        GameEvent getStat = GameEventPool.Get(GameEventId.GetStatRaw)
                                .With(EventParameters.StatType, ControlledStat)
                                .With(EventParameters.Value, Stat.Str);

        int value = source.FireEvent(getStat).GetValue<int>(EventParameters.Value);
        Text.text = $"{ControlledStat.ToString()}: {value}";
        getStat.Release();
    }

    public void UpdateUI(IEntity newSource)
    {
        Setup(newSource);
    }
}
