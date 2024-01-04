using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputControllerBase : EntityComponent
{
    public bool? AlternativeEscapeKeyPressed => false;

    public void OnEscape()
    {
        //UIManager.ForcePop(this);
    }
}
