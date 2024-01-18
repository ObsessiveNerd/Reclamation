using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class GameObjectExtensions
{
    public static GameEvent FireEvent(this GameObject source, GameObject target, GameEvent gameEvent, bool logEvent = false)
    {
        if(target != null)
        {
            var entity = target.GetComponent<EntityBehavior>().Entity;
            foreach(var comp in entity.GetComponents())
                comp.HandleEvent(gameEvent);
        }
        return gameEvent;
    }

    public static GameEvent FireEvent(this GameObject source, GameEvent gameEvent, bool logEvent = false)
    {
        if(source != null)
        {
            var entity = source.GetComponent<EntityBehavior>().Entity;
            foreach(var comp in entity.GetComponents())
                comp.HandleEvent(gameEvent);
        }
        return gameEvent;
    }
    //public static void WakeUp(this GameObject source)
    //{
    //    var components = source.GetComponents<EntityComponentBehavior>();
    //    foreach (var component in components)
    //        component.GetData()?.WakeUp();
    //}

    public static void Show(this GameObject source)
    {
        SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
        if(sr != null)
            sr.enabled = true;

        BoxCollider2D collider = source.GetComponent<BoxCollider2D>();
        if(collider != null)
            collider.enabled = true;
    }

    public static void Hide(this GameObject source)
    {
        SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
        if(sr != null)
            sr.enabled = false;

        BoxCollider2D collider = source.GetComponent<BoxCollider2D>();
        if(collider != null)
            collider.enabled = false;
    }
}
