using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTab : MonoBehaviour
{
    public Image m_Portrait;
    public TextMeshProUGUI m_PrettyName;

    public void Setup(Sprite portrait, string name)
    {
        m_PrettyName = GetComponentInChildren<TextMeshProUGUI>();

        m_Portrait.sprite = portrait;
        m_PrettyName.text = name;
    }
}
