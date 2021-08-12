using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

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
    public Sprite Portrait = null;
    public Sprite Icon = null;
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

    public void AddComponent(BlueprintValues bpv, IComponent comp)
    {
        if(comp.GetType() == typeof(GraphicContainer))
        {
            string sp = ((GraphicContainer)comp).SpritePath;
            //if (sp.EndsWith(".png"))
            //    sp = sp.Replace(".png", "");

            Icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{sp}.png");
        }
        else if(comp.GetType() == typeof(Portrait))
        {
            string sp = ((Portrait)comp).SpritePath;
            //if (sp.EndsWith(".png"))
            //    sp = sp.Replace(".png", "");
            Portrait = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{sp}.png");
        }
        else
        {
            foreach (var field in comp.GetType().GetFields())
            {
                if (comp.GetType() == typeof(Body))
                {
                    bpv.FieldToValue.Add(field.Name, comp.ToString());
                    break;
                    //if (field.FieldType == typeof(List<IEntity>))
                    //{
                    //    var value = field.GetValue(comp);
                    //    if (value != null)
                    //        bpv.FieldToValue.Add(field.Name, (value as List<IEntity>).Count.ToString());
                    //    else
                    //        bpv.FieldToValue.Add(field.Name, "0");
                    //}
                    //else
                    //    bpv.FieldToValue.Add(field.Name, "1");
                }
                else
                {
                    string value = "";
                    if (field.FieldType == typeof(List<IEntity>))
                        value = EntityFactory.ConvertEntitiesToStringArrayWithName(field.GetValue(comp) as List<IEntity>);
                    else if (field.FieldType == typeof(Dictionary<string, IEntity>))
                    {
                        value = EntityFactory.ConvertEntitiesToStringArrayWithName((field.GetValue(comp) as Dictionary<string, IEntity>)?.Values.ToList());
                    }
                    else
                        value = field.GetValue(comp)?.ToString();
                    bpv.FieldToValue.Add(field.Name, value);
                }
            }
        }
        Components.Add(bpv);
    }

    public string CreateNewBlueprint()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<{BlueprintName}> (");

        if (Icon != null)
            sb.AppendLine($"\tGraphicContainer: {AssetDatabase.GetAssetPath(Icon).Replace("Assets/Resources/","").Replace(".png", "")}");
        if(Portrait != null)
            sb.AppendLine($"\tPortrait: {AssetDatabase.GetAssetPath(Portrait).Replace("Assets/Resources/","").Replace(".png", "")}");

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