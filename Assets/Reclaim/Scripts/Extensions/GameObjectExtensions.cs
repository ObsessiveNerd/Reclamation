using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameEvent FireEvent(this GameObject source, GameObject target, GameEvent gameEvent, bool logEvent = false)
    {
        if(target != null)
        {
            foreach(var comp in target.GetComponents<EntityComponent>())
                comp.HandleEvent(gameEvent);
        }
        return gameEvent;
    }

    public static GameEvent FireEvent(this GameObject source, GameEvent gameEvent, bool logEvent = false)
    {
        if(source != null)
        {
            foreach(var comp in source.GetComponents<EntityComponent>())
                comp.HandleEvent(gameEvent);
        }
        return gameEvent;
    }

    public static bool HasComponent<T>(this GameObject source) where T : EntityComponent
    {
        return source.GetComponent<T>() != null;
    }

    public static string Serialize(this GameObject source)
    {
        SerializableGameObject item = new SerializableGameObject(source);
        var data = JsonUtility.ToJson(item);

        GameObject go = new GameObject();
        foreach (var i in item.Data)
        {
            var t = JsonUtility.FromJson(i.Data, i.ComponentType) as EntityComponent;
            var added = go.AddComponent(i.ComponentType);
            foreach(var field in i.ComponentType.GetFields(System.Reflection.BindingFlags.Public))
            {
                field.SetValue(added, field.GetValue(t));
            }
            GameObject.Instantiate(go);
        }

        return data;
    }

    public static void Show(this GameObject source)
    {
        SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
        if(sr != null)
            sr.enabled = true;

        BoxCollider2D collider = source.GetComponent<BoxCollider2D>();
        if(collider != null)
            collider.enabled = true;
    }

    public static void WakeUp(this GameObject source)
    {
        var components = source.GetComponents<EntityComponent>();
        foreach (var component in components)
            component.GetData()?.WakeUp();
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
