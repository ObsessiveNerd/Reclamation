﻿#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum BlueprintArchetype
{
    None = 0,
    Creature,
    Item,
    Object
}

public class BlueprintValues
{
    public string ComponentName;
    public int ComponentNameIndex;
    public bool Foldout;
    public Dictionary<string, string> FieldToValue = new Dictionary<string, string>();
}

public class BlueprintCreator
{
    public string BlueprintName;
    public List<BlueprintValues> Components;
    //public Sprite Portrait = null;
    //public Sprite Icon = null;
    public BlueprintArchetype Archetype;

    public BlueprintCreator()
    {
        BlueprintName = "";
        Components = new List<BlueprintValues>();
    }

    public BlueprintCreator(string name)
    {
        BlueprintName = name;
        Components = new List<BlueprintValues>();
    }

    public void AddComponent(BlueprintValues bpv, Type comp, string[] data)
    {
        Dictionary<string, string> kvp = new Dictionary<string, string>();
        if (data == null)
        {
            foreach(var field in comp.GetFields())
                kvp.Add(field.Name, "");
        }
        else if (comp == typeof(GraphicContainer) || comp == typeof(Portrait))
        {
            kvp.Add("SpritePath", data[0]);
        }
        else
        {
            foreach (var s in data)
            {
                if(s.Split('=').Length == 1)
                    kvp.Add(s.Split('=')[0], "");
                else
                    kvp.Add(s.Split('=')[0], s.Split('=')[1]);
            }
        }

        //if(comp.GetType() == typeof(GraphicContainer))
        //{
        //    string sp = ((GraphicContainer)comp).SpritePath;
        //    Icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{sp}.png");
        //}
        //else if(comp.GetType() == typeof(Portrait))
        //{
        //    string sp = ((Portrait)comp).SpritePath;
        //    Portrait = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{sp}.png");
        //}
        //else
        {
            foreach (var field in comp.GetFields())
            {

                if (!kvp.ContainsKey(field.Name))
                    bpv.FieldToValue.Add(field.Name, "");
                else
                    bpv.FieldToValue.Add(field.Name, kvp[field.Name]);
            }
        }
        Components.Add(bpv);
    }

    public string CreateNewBlueprint()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<{BlueprintName}> (");

        //if (Icon != null)
        //    sb.AppendLine($"\tGraphicContainer: {AssetDatabase.GetAssetPath(Icon).Replace("Assets/Resources/","").Replace(".png", "")}");
        //if(Portrait != null)
        //    sb.AppendLine($"\tPortrait: {AssetDatabase.GetAssetPath(Portrait).Replace("Assets/Resources/","").Replace(".png", "")}");

        foreach (var comp in Components)
        {
            string componentString = $"\t{comp.ComponentName}:";
            if (comp.FieldToValue.Count == 0)
                componentString = componentString.TrimEnd(':');
            else
            {
                if (comp.ComponentName == nameof(Body))
                    componentString += comp.FieldToValue["Torso"];
                else
                {
                    foreach (var keyValuePair in comp.FieldToValue)
                        componentString += $"{keyValuePair.Key}={keyValuePair.Value},";
                }
                componentString = componentString.TrimEnd(',');
            }
            sb.AppendLine(componentString);
        }
        sb.AppendLine(")");
        return sb.ToString();
    }
}
#endif