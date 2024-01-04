using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : InputControllerBase
{
    public void Update()
    {
        if (UIManager.UIClear)
        {
            gameObject.AddComponent<PlayerInputController>();
            Destroy(this);
        }
    }
}