using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonLevelUI : MonoBehaviour
{
    public TextMeshProUGUI Level;

    // Update is called once per frame
    void Update()
    {
        Level.text = $"{WorldComponent.CurrentLevel}";
    }
}
