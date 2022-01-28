using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerInfoMono : MonoBehaviour
{
    public TextMeshProUGUI Info;

    public void Setup(IEntity source)
    {
        StringBuilder sb = new StringBuilder();

        GameEvent getArmor = GameEventPool.Get(GameEventId.AddArmorValue)
                                .With(EventParameters.Value, 0);
        source.FireEvent(getArmor);

        sb.AppendLine($"Armor: {getArmor.GetValue<int>(EventParameters.Value)}");
        getArmor.Release();

        sb.AppendLine("");

        GameEvent getResistances = GameEventPool.Get(GameEventId.GetResistances)
                                    .With(EventParameters.Resistances, new List<DamageType>());
        source.FireEvent(getResistances);
        List<DamageType> resistances = getResistances.GetValue<List<DamageType>>(EventParameters.Resistances);
        if(resistances.Count > 0)
        {
            sb.AppendLine("Resistances:");
            foreach (var dt in resistances)
                sb.AppendLine(dt.ToString());
        }
        getResistances.Release();

        sb.AppendLine("");

        GameEvent getImmunities = GameEventPool.Get(GameEventId.GetImmunity)
                                    .With(EventParameters.Immunity, new List<DamageType>());
        source.FireEvent(getImmunities);
        List<DamageType> immunities = getImmunities.GetValue<List<DamageType>>(EventParameters.Immunity);
        if(resistances.Count > 0)
        {
            sb.AppendLine("Immunity:");
            foreach (var dt in immunities)
                sb.AppendLine(dt.ToString());
        }
        getImmunities.Release();

        Info.text = sb.ToString();
    }

    public void Close()
    {

    }
}
