using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsMono : UpdatableUI//, IUpdatableUI
{
    //public TextMeshProUGUI Name;
    public TextMeshProUGUI AttributePoints;
    List<StatsUIMono> StatMonos;

    public override void UpdateUI()
    {
        IEntity source = Services.PlayerManagerService.GetActivePlayer();

        StatMonos = GetComponentsInChildren<StatsUIMono>().ToList();
        GameEvent getAttributePoints = GameEventPool.Get(GameEventId.GetAttributePoints)
                                            .With(EventParameters.AttributePoints, 0);
        int attrPoints = source.FireEvent(getAttributePoints).GetValue<int>(EventParameters.AttributePoints);
        getAttributePoints.Release();

        if (attrPoints == 0)
        {
            AttributePoints.gameObject.SetActive(false);
            foreach(var statMono in StatMonos)
            {
                statMono.Button.gameObject.SetActive(false);
                statMono.Button.onClick.RemoveAllListeners();
                statMono.SetData();
            }
        }
        else
        {
            AttributePoints.gameObject.SetActive(true);
            AttributePoints.text = $"Attribute Points: {attrPoints}";

            foreach(var statMono in StatMonos)
            {
                statMono.Button.onClick.RemoveAllListeners();
                statMono.Button.gameObject.SetActive(true);
                statMono.Button.onClick.AddListener(() =>
                {
                    GameEvent boostStat = GameEventPool.Get(GameEventId.BoostStat)
                                                .With(EventParameters.StatType, statMono.ControlledStat)
                                                .With(EventParameters.StatBoostAmount, 1);
                    source.FireEvent(boostStat);
                    boostStat.Release();
                });
                statMono.Button.onClick.AddListener(() => UpdateUI());
                statMono.SetData();
            }
        }
    }
}
