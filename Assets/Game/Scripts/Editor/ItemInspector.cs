using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            var item = serializedObject.targetObject as Item;
            if(item != null && item.AssignedItem != null)
                item.GetComponent<SpriteRenderer>().sprite = item.AssignedItem.Sprite;
        }
    }
} 
