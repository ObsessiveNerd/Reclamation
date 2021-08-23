using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterStatsMono : MonoBehaviour, IUpdatableUI
{
    public TextMeshProUGUI Name;

    public void Setup(IEntity source)
    {
        Name.text = source.Name;
        WorldUtility.RegisterUI(this);
    }

    public void Cleanup()
    {
        Name.text = "";
    }

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        Setup(newSource);
    }

    public void Close()
    {
        Cleanup();
        WorldUtility.UnRegisterUI(this);
    }
}
