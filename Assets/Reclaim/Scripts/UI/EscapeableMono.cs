using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeableMono : MonoBehaviour
{
    public virtual void OnEscape() { }
    public virtual bool? AlternativeEscapeKeyPressed{ get { return false; } }
}
