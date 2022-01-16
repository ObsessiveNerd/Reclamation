using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using static EntityFactory;

public class ContentCreationWindow : EditorWindow
{
    BlueprintCreator m_Creator;
    Type[] m_ComponentTypes;
    List<string> m_ComponentNames;

    [MenuItem("Reclamation/Blueprint Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<ContentCreationWindow>();
        window.Setup();
        window.Show();
    }

    public void Setup()
    {
        m_Creator = new BlueprintCreator();
        var type = typeof(Component);
        m_ComponentNames = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Select(p => p.AssemblyQualifiedName)
            .ToList();
        m_ComponentNames.Sort();

        m_ComponentTypes = m_ComponentNames.Select(p => Type.GetType(p)).ToArray();
        LoadArchetype();
    }

    Vector2 m_CurrentScrollPos = Vector2.zero;
    bool m_ShowFullComponentList = false;

    string m_SearchString = "";

    void OnGUI()
    {
        if (m_Creator == null)
            Setup();

        m_Creator.BlueprintName = EditorGUILayout.TextField(new GUIContent("Blueprint Name"), m_Creator.BlueprintName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Blueprint Archetype:");
        EditorGUI.BeginChangeCheck();
        m_Creator.Archetype = (BlueprintArchetype)EditorGUILayout.Popup((int)m_Creator.Archetype, Enum.GetNames(typeof(BlueprintArchetype)));
        if(EditorGUI.EndChangeCheck())
        {
            m_Creator.Components.Clear();
            LoadArchetype();
        }
        EditorGUILayout.Space();

        //EditorGUILayout.BeginHorizontal();
        //m_Creator.Portrait = (Sprite)EditorGUILayout.ObjectField("Portrait", m_Creator.Portrait, typeof(Sprite), false);
        //m_Creator.Icon = (Sprite)EditorGUILayout.ObjectField("Map Icon", m_Creator.Icon, typeof(Sprite), false);
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        m_SearchString = EditorGUILayout.TextField("Search", m_SearchString);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(1));
        m_ShowFullComponentList = EditorGUILayout.Toggle(m_ShowFullComponentList);
        EditorGUILayout.LabelField("Full Component List");
        EditorGUILayout.EndHorizontal();

        m_CurrentScrollPos = EditorGUILayout.BeginScrollView(m_CurrentScrollPos);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Components");
        if (GUILayout.Button("Add Component"))
        {
            GenericMenu gm = new GenericMenu();
            for(int i = 0; i < m_ComponentTypes.Count(); i++)
                gm.AddItem(new GUIContent(m_ComponentTypes[i].Name), false, (value) =>
                {
                    m_Creator.AddComponent(new BlueprintValues()
                    {
                        ComponentName = m_ComponentTypes[(int)value].Name,
                        ComponentNameIndex = (int)value
                    }, m_ComponentTypes[(int)value], null);
                }, i);
            gm.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        GuiLine();
        List<BlueprintValues> componenetsToRemove = new List<BlueprintValues>();
        foreach (var component in m_Creator.Components.Where(c =>
        {

            if (string.IsNullOrEmpty(m_SearchString))
                return true;
            else
                return c.ComponentName.ToLower().Contains(m_SearchString.ToLower());
        }))
        {
            if (component == null)
                continue;

            if (!m_ShowFullComponentList && m_ComponentTypes[component.ComponentNameIndex].GetFields().Count() == 0)
                continue;

            EditorGUILayout.BeginHorizontal();
            component.ComponentNameIndex = EditorGUILayout.Popup(component.ComponentNameIndex, m_ComponentTypes.Select(p => p.Name).ToArray());
            if(GUILayout.Button("Remove"))
            {
                componenetsToRemove.Add(component);
                continue;
            }
            EditorGUILayout.EndHorizontal();
            Type t = m_ComponentTypes[component.ComponentNameIndex];
            FieldInfo[] fields = t.GetFields();
            component.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(component.Foldout, m_ComponentTypes[component.ComponentNameIndex].Name);
            if(component.Foldout)
            {
                foreach (var fieldInfo in fields)
                {
                    if (!component.FieldToValue.ContainsKey(fieldInfo.Name))
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(fieldInfo.Name);
                    if (fieldInfo.FieldType.IsEnum)
                    {
                        var enumValues = Enum.GetValues(fieldInfo.FieldType);
                        List<string> values = new List<string>();
                        foreach (var e in enumValues)
                            values.Add(e.ToString());

                        var value = component.FieldToValue[fieldInfo.Name];
                        int index = 0;
                        for (int i = 0; i < enumValues.Length; i++)
                        {
                            if (values[i] == value.ToString())
                            {
                                index = i;
                                break;
                            }
                        }

                        component.FieldToValue[fieldInfo.Name] = values[EditorGUILayout.Popup(index, values.ToArray())];
                    }
                    else if(fieldInfo.FieldType == typeof(SoundPath))
                    {
                        component.FieldToValue[fieldInfo.Name] = AssetDatabase.GetAssetPath(EditorGUILayout.ObjectField("Sound",
                            AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Resources/{component.FieldToValue[fieldInfo.Name]}"), typeof(AudioClip), false)).Replace("Assets/Resources/", "");
                    }
                    else
                    {
                        component.FieldToValue[fieldInfo.Name] = EditorGUILayout.TextField(component.FieldToValue[fieldInfo.Name]);
                        var obj = EditorGUILayout.ObjectField("", null, typeof(UnityEngine.Object), false);
                        if (obj != null)
                            component.FieldToValue[fieldInfo.Name] = AssetDatabase.GetAssetPath(obj).Replace("Assets/Resources/", "").Split('.')[0];
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GuiLine();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

        foreach (var comp in componenetsToRemove)
            m_Creator.Components.Remove(comp);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save As"))
            SaveEntityAs();
        if (GUILayout.Button("Override"))
            OverrideEntity();
        if (GUILayout.Button("Clear"))
            Clear();
        if(GUILayout.Button("Load"))
        {
            string path = EditorUtility.OpenFilePanel("Load Blueprint", $"{Application.dataPath}/../Blueprints", "bp");
            string fileContents = File.ReadAllText(path);
            EntityData e = EntityFactory.GetEntityDataWithoutCreation(fileContents);
            CreateNewBPBuilder(e.Name);
            foreach (var comp in e.Components)
            {
                var bpValue = new BlueprintValues()
                {
                    ComponentName = comp.ComponentType.ToString(), //.GetType().ToString(),
                    ComponentNameIndex = GetTypeIndex(comp.ComponentType.ToString())
                };
                m_Creator.AddComponent(bpValue, comp.ComponentType, comp.KeyValuePairs);

                if (comp.GetType() == typeof(GraphicContainer) || comp.GetType() == typeof(Portrait))
                    m_Creator.Components.Remove(bpValue);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    int GetTypeIndex(string componentName)
    {
        for(int i = 0; i < m_ComponentTypes.Count(); i++)
        {
            if (m_ComponentTypes[i].Name == componentName)
                return i;
        }
        return 0;
    }

    void CreateNewBPBuilder(string name = "")
    {
        m_Creator = new BlueprintCreator(name);
        LoadArchetype();
    }

    void Clear()
    {
        m_Creator = new BlueprintCreator();
        LoadArchetype();
    }

    void LoadArchetype()
    {
        if (m_Creator.Archetype == BlueprintArchetype.None)
            return;

        //IEntity archetype = EntityFactory.GetEntity($"{Application.dataPath}/../Blueprints/Architype/{m_Creator.Archetype.ToString()}.bp", "");
        var archetype = EntityFactory.GetEntityDataWithoutCreation(File.ReadAllText($"{Application.dataPath}/../Blueprints/Architype/{m_Creator.Archetype.ToString()}.bp"));
        foreach (var comp in archetype.Components)
        {
            m_Creator.AddComponent(new BlueprintValues()
            {
                ComponentName = comp.ComponentType.ToString(),
                ComponentNameIndex = GetTypeIndex(comp.ComponentType.ToString())
            }, comp.ComponentType, comp.KeyValuePairs);
        }
    }

    void OverrideEntity()
    {
        string path = EditorUtility.OpenFilePanel("Blueprint Folder", $"{Application.dataPath}/../Blueprints/", "bp");
        string blueprint = m_Creator.CreateNewBlueprint();
        File.Delete(path);
        File.WriteAllText(path, blueprint);
        Clear();
    }

    void SaveEntityAs()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Blueprint Folder", $"{Application.dataPath}/../Blueprints/", "");
        string path = $"{folderPath}/{m_Creator.BlueprintName}.bp";
        string blueprint = m_Creator.CreateNewBlueprint();
        File.WriteAllText(path, blueprint);
        Clear();
    }

    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
