using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeableMono : UpdatableUI
{
    protected bool m_OpenedThisFrame = false;
    
    protected override void OnEnable()
    {
        UIManager.Push(this);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        UIManager.ForcePop(this);
        base.OnDisable();
    }

    protected void OpenedThisFrame()
    {
        m_OpenedThisFrame = true;
    }

    private void LateUpdate()
    {
        m_OpenedThisFrame = false;
    }

    public virtual void OnEscape() { }
    public virtual bool? AlternativeEscapeKeyPressed{ get { return false; } }
}
