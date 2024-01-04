using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatableUI
{
    void UpdateUI();
}

public class UpdatableUI : MonoBehaviour, IUpdatableUI
{
    protected virtual void OnEnable()
    {
        //if (!Services.Ready)
        //    Services.InitComplete += (sender, evnt) => Services.WorldUIService.RegisterUpdatableUI(this);
        //else
        //    Services.WorldUIService.RegisterUpdatableUI(this);
    }

    protected virtual void OnDisable()
    {
        //if(!Services.Ready)
        //    Services.InitComplete += (sender, evnt) => Services.WorldUIService.UnregisterUpdatableUI(this);
        //else
        //    Services.WorldUIService.UnregisterUpdatableUI(this);
    }

    public virtual void UpdateUI() { }
}
