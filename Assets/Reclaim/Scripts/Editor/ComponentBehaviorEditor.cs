using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(ComponentBehavior<>), true)]
public class ComponentBehaviorEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    var feature = target as IComponentBehavior;
    //    var component = feature.GetComponent();

    //    var fields = component.GetType().GetFields();
    //    foreach (var field in fields)
    //    {
    //        if (field.FieldType == typeof(bool))
    //            field.SetValue(component, EditorGUILayout.Toggle((bool)field.GetValue(component)));

    //        else if (field.FieldType == typeof(float))
    //            field.SetValue(component, EditorGUILayout.FloatField((float)field.GetValue(component)));

    //        else if (field.FieldType == typeof(int))
    //            field.SetValue(component, EditorGUILayout.IntField((int)field.GetValue(component)));

    //        else if (field.FieldType == typeof(string))
    //            field.SetValue(component, EditorGUILayout.TextField((string)field.GetValue(component)));

    //        else if (field.FieldType.IsAssignableFrom(typeof(UnityEngine.Object)))
    //            field.SetValue(component, EditorGUILayout.ObjectField((Object)field.GetValue(component), typeof(Object), true));
    //    }

    //    base.OnInspectorGUI();
    //}
}
