using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatableUI
{
    void UpdateUI(IEntity newSource);
}

public class UpdatableUI : MonoBehaviour//, IUpdatableUI
{
    void Start()
    {
        //Services.WorldUIService.RegisterUpdatableUI(this);
    }

    void OnDisable()
    {
        //Services.WorldUIService.UnregisterUpdatableUI(this);
    }

    public virtual void UpdateUI() { }
}
