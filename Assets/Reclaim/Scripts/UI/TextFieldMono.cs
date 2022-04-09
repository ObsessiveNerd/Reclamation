using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFieldMono : MonoBehaviour
{
    public TextMeshProUGUI Placeholder;

    public void Init(string defaultText)
    {
        Placeholder.text = defaultText;
    }
}
