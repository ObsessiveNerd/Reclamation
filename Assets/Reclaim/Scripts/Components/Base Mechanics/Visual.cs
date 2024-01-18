using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisualData : EntityComponent
{
    public int RenderLayer;
    public Sprite MapSprite;
    public Sprite Portrait;

    public override Type MonobehaviorType => typeof(Visual);
}

public class Visual : ComponentBehavior<VisualData>
{
    void Start()
    {
        //Will have to redo this later
        GetComponent<SpriteRenderer>().sortingOrder = component.RenderLayer;
        GetComponent<SpriteRenderer>().sprite = component.MapSprite;
    }
}
