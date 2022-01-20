using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class EntityFactory
{
    private static List<string> s_EnvironmentEntities = new List<string>();
    public static List<string> EnvironmentEntities
    {
        get
        {
            if (s_EnvironmentEntities == null)
                s_EnvironmentEntities = new List<string>();
            if (m_Blueprints.Count == 0)
                InitBlueprints();
            return s_EnvironmentEntities;
        }
    }

    private static List<string> s_InventoryEntities = new List<string>();
    public static List<string> InventoryEntities
    {
        get
        {
            if (s_InventoryEntities == null)
                s_InventoryEntities = new List<string>();
            if (m_Blueprints.Count == 0)
                InitBlueprints();
            return s_InventoryEntities;
        }
    }

    private static string m_BluePrintPath = "Blueprints";
    private static Dictionary<string, string> m_Blueprints = new Dictionary<string, string>();

    private static bool s_InitializingBluePrints = false;
    private static Dictionary<string, List<string>> m_BlueprintTypeMap = new Dictionary<string, List<string>>();
    private static Dictionary<string, List<string>> BlueprintTypeMap
    {
        get
        {
            if (m_BlueprintTypeMap.Count == 0 && !s_InitializingBluePrints)
                InitBlueprints();
            return m_BlueprintTypeMap;
        }
    }

    private static bool m_LoadedTempBlueprints = false;

#pragma warning disable 0414
    private static string kArmorPath = "Blueprints\\Armor";
    private static string kEnvironmentPath = "Blueprints\\Environment";
    private static string kWeaponPath = "Blueprints\\Weapons";
    private static string kMonstersPath = "Blueprints\\Monsters";
    private static string kBossesPath = "Blueprints\\Boss";
    private static string kSpellsPath = "Blueprints\\Spells";
    private static string kObjectsPath = "Blueprints\\Objects";
    private static string kItemsPath = "Blueprints\\Items";
#pragma warning restore 0414

    public static IEntity CreateEntity(string blueprintName)
    {
        return GetEntity(blueprintName);
    }

    private static void InitBlueprints()
    {
        s_InitializingBluePrints = true;
        foreach (var bpPath in Directory.EnumerateFiles(m_BluePrintPath, "*", SearchOption.AllDirectories))
        {
            string bpName = Path.GetFileNameWithoutExtension(bpPath);
            if (m_Blueprints.ContainsKey(bpName))
                continue;

            string directoryOnly = Path.GetDirectoryName(bpPath);
            if (!BlueprintTypeMap.ContainsKey(directoryOnly))
                BlueprintTypeMap.Add(directoryOnly, new List<string>());
            BlueprintTypeMap[directoryOnly].Add(bpName);
            
            m_Blueprints.Add(bpName, bpPath);

            if (bpPath.StartsWith(kArmorPath) || bpPath.StartsWith(kWeaponPath) || bpPath.StartsWith(kItemsPath))
                s_InventoryEntities.Add(bpName);

            if (bpPath.StartsWith(kEnvironmentPath))
                s_EnvironmentEntities.Add(bpName);
        }
    }

    public static void ReloadTempBlueprints()
    {
        m_LoadedTempBlueprints = false;
        InitTempBlueprints();
    }

    public static void Clean()
    {
        m_LoadedTempBlueprints = false;
        s_InitializingBluePrints = false;
        s_InventoryEntities.Clear();
        m_Blueprints.Clear();
        m_BlueprintTypeMap.Clear();
    }

    public static void InitTempBlueprints()
    {
        if (World.Services != null && !m_LoadedTempBlueprints && SaveSystem.Instance != null)
        {
            string tempBlueprints = $"{SaveSystem.kSaveDataPath}/{SaveSystem.Instance.CurrentSaveName}/Blueprints";
            if (Directory.Exists(tempBlueprints))
            {
                foreach (var bpPath in Directory.EnumerateFiles(tempBlueprints, "*", SearchOption.AllDirectories))
                {
                    string bpName = Path.GetFileNameWithoutExtension(bpPath);
                    if(!m_Blueprints.ContainsKey(bpName))
                        m_Blueprints.Add(bpName, bpPath);

                    //CreateEntity(bpName);
                    if(int.TryParse(bpName, out int res))
                        IDManager.SetId(res);
                    else if(int.TryParse(ParseEntityIdFromName(bpName), out int res2))
                        IDManager.SetId(res2);
                }
                m_LoadedTempBlueprints = true;
            }
        }
    }

    public static string GetRandomEnvironmentBPName()
    {
        return EnvironmentEntities[RecRandom.Instance.GetRandomValue(0, s_EnvironmentEntities.Count)];
    }

    public static string GetRandomCharacterName()
    {
        var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/random_names.txt");

        var randomLineNumber = RecRandom.Instance.GetRandomValue(0, lines.Length - 1);
        var line = lines[randomLineNumber];

        return line;
    }

    public static string GetRandomMonsterBPName()
    {
        var list = BlueprintTypeMap[kMonstersPath];
        return list[RecRandom.Instance.GetRandomValue(0, list.Count)];
    }

    public static string GetRandomItemBPName(int rarity)
    {
        return string.Empty;
    }

    public static string GetRandomSpellBPName(int rarity)
    {
        var list = BlueprintTypeMap[kSpellsPath];
        return list[RecRandom.Instance.GetRandomValue(0, list.Count)];
    }

    public static IEntity GetEntity(string blueprintName /*string path, string entityID = ""*/)
    {
        if(m_Blueprints.Count == 0)
            InitBlueprints();

        InitTempBlueprints();

        if (!m_Blueprints.ContainsKey(blueprintName))
            return null;

        Actor a = new Actor("<empty>");
        string path = m_Blueprints[blueprintName]; //$"{m_BluePrintPath}/{blueprintName}.bp";
        if(!File.Exists(path))
            path = $"{SaveSystem.kSaveDataPath}/{World.Services.Seed}/Blueprints/{blueprintName}.bp"; //todo: need proper seed
        if (!File.Exists(path))
            return null;

        using (var stream = new StreamReader(path))
        {
            string header = stream.ReadLine();
            int firstIndex = header.IndexOf('<');
            int lastIndex = header.IndexOf('>');
            

            int nameStart = firstIndex + 1;
            int nameLength = lastIndex - firstIndex - 1;
            string name = header.Substring(nameStart, nameLength);

            string[] nameAndID = name.Split(',');
            if (nameAndID.Length == 1)
                a = new Actor(name, a.ID);
            else
                a = new Actor(nameAndID[0], nameAndID[1]);

            //a = new Actor(name, entityID);

            string line;
            while ((line = stream.ReadLine()) != null && line != ")")
            {
                string[] componentToValues = line.Split(':');
                string dtoComponentName = $"DTO_{componentToValues[0].Replace("\t", string.Empty)}";
                Type componentType = Type.GetType(dtoComponentName);
                if (componentType == typeof(DTO_Inherits))
                {
                    string data = componentToValues[1].Replace(" ", string.Empty);
                    DTO_Inherits inherit = new DTO_Inherits();
                    a.AddComponentRange(inherit.CreateComponents($"{m_BluePrintPath}/Architype", data));
                }
                else if (componentType != null)
                {
                    IDataTransferComponent dtc = (IDataTransferComponent)Activator.CreateInstance(componentType);
                    if (componentToValues.Length == 1)
                        dtc.CreateComponent("");
                    else
                    {
                        string data = componentToValues[1];
                        if (!data.StartsWith("\"") && !data.EndsWith("\""))
                            data = componentToValues[1].Replace(" ", string.Empty);
                        dtc.CreateComponent(data);
                    }
                    a.AddComponent(dtc.Component);
                }
                else
                    Debug.LogError($"Couldn't find componentType for {dtoComponentName}");
            }
        }
        a.CleanupComponents();


        //GameEvent register = GameEventPool.Get(GameEventId.RegisterEntity)
        //                            .With(EventParameters.Entity, a);
        //World.Instance.Self.FireEvent(register.CreateEvent());

        //foreach (var comp in a.GetComponents())
        //    comp.Start();
        return a;
    }

    public struct ComponentData
    {
        public Type ComponentType;
        public string[] KeyValuePairs;

        public ComponentData(Type t, string[] data)
        {
            ComponentType = t;
            KeyValuePairs = data;
        }
    }

    public struct EntityData
    {
        public string Name;
        public List<ComponentData> Components;
    }

    public static EntityData GetEntityDataWithoutCreation(string entityData)
    {
        string name = "";
        List<ComponentData> cd = new List<ComponentData>();
        using (var stream = new StringReader(entityData))
        {
            string header = stream.ReadLine();
            int firstIndex = header.IndexOf('<');
            int lastIndex = header.IndexOf('>');

            int nameStart = firstIndex + 1;
            int nameLength = lastIndex - firstIndex - 1;
            name = header.Substring(nameStart, nameLength);

            string line;
            while ((line = stream.ReadLine()) != null && line != ")")
            {
                string[] componentToValues = line.Split(':');
                string componentName = $"{componentToValues[0].Replace("\t", string.Empty)}";
                Type componentType = Type.GetType(componentName);

                if (componentToValues.Length > 1)
                    cd.Add(new ComponentData(componentType, componentToValues[1].Split(',')));
                else
                    cd.Add(new ComponentData(componentType, null));

            }
        }
        return new EntityData
        {
            Name = name,
            Components = cd
        };
    }

    public static IEntity ParseEntityData(string entityData)
    {
        Actor a = null;
        using (var stream = new StringReader(entityData))
        {
            string header = stream.ReadLine();
            int firstIndex = header.IndexOf('<');
            int lastIndex = header.IndexOf('>');

            int nameStart = firstIndex + 1;
            int nameLength = lastIndex - firstIndex - 1;
            string name = header.Substring(nameStart, nameLength);

            string[] nameAndID = name.Split(',');
            if (nameAndID.Length == 1)
                a = new Actor(name);
            else
                a = new Actor(nameAndID[0], nameAndID[1]);

            string line;
            while ((line = stream.ReadLine()) != null && line != ")")
            {
                string[] componentToValues = line.Split(':');
                string dtoComponentName = $"DTO_{componentToValues[0].Replace("\t", string.Empty)}";
                Type componentType = Type.GetType(dtoComponentName);
                if (componentType == typeof(DTO_Inherits))
                {
                    string data = componentToValues[1].Replace(" ", string.Empty);
                    DTO_Inherits inherit = new DTO_Inherits();
                    a.AddComponentRange(inherit.CreateComponents($"{m_BluePrintPath}/Architype", data));
                }
                else if (componentType != null)
                {
                    IDataTransferComponent dtc = (IDataTransferComponent)Activator.CreateInstance(componentType);
                    if (componentToValues.Length == 1)
                        dtc.CreateComponent("");
                    else
                    {
                        string data = componentToValues[1];
                        if (!data.StartsWith("\"") && !data.EndsWith("\""))
                            data = componentToValues[1].Replace(" ", string.Empty);
                        dtc.CreateComponent(data);
                    }
                    a.AddComponent(dtc.Component);
                }
                else
                    Debug.LogError($"Couldn't find componentType for {dtoComponentName}");
            }
        }
        a.CleanupComponents();
        //foreach (var comp in a.GetComponents())
        //    comp.Start();
        return a;
    }

    public static List<IEntity> GetEntitiesFromArray(string data)
    {
        InitBlueprints();
        InitTempBlueprints();

        if (string.IsNullOrEmpty(data))
            return new List<IEntity>();

        List<IEntity> result = new List<IEntity>();
        data = data.Replace(" ", string.Empty).TrimStart('[').TrimEnd(']');
        string[] parameters = data.Split('&');
        foreach(string parameter in parameters)
        {
            if (string.IsNullOrEmpty(parameter))
                continue;

            string value = parameter;
            if (value.Contains("="))
                value = value.Split('=')[1];
            string entityName = GetEntityNameFromBlueprintFormatting(value);

            var en = CreateEntity(entityName);
            if (en != null)
                result.Add(en);
            else
                Debug.LogError($"Issue creating entity ({entityName}): {value} from param: {parameter}");
        }
        return result;
    }

    public static string ConvertEntitiesToStringArrayWithId(List<IEntity> entities)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var entity in entities)
            if(entity != null)
                sb.Append($"<{entity.ID}>&");

        return sb.ToString().TrimEnd('&');
    }

    public static string ConvertEntitiesToStringArrayWithName(List<IEntity> entities)
    {
        if (entities == null)
            return "";

        StringBuilder sb = new StringBuilder();
        foreach (var entity in entities)
            if(entity != null)
                sb.Append($"<{entity.Name}>&");

        return sb.ToString().TrimEnd('&');
    }

    public static string GetEntityNameFromBlueprintFormatting(string bpFormatting)
    {
        int start = bpFormatting.IndexOf('<') + 1;
        int length = bpFormatting.IndexOf('>') - start;
        if(length > start)
            return bpFormatting.Substring(start, length);
        return bpFormatting;
    }

    public static string ParseEntityIdFromName(string name)
    {
        return GetEntityNameFromBlueprintFormatting(name).Split(',')[0];
    }

    public static void CreateTemporaryBlueprint(string blueprintName, string data)
    {
        //if (!int.TryParse(blueprintName, out int result))
        //    Debug.Log("bad");

        string path = $"Blueprints/{blueprintName}.bp";
        SaveSystem.Instance.WriteData(path, data);
    }
}

public class DTO_Inherits
{
    public List<IComponent> CreateComponents(string architypePath, string data)
    {
        List<IComponent> retValue = new List<IComponent>();
        string[] parameters = data.Split(',');
        foreach(string parameter in parameters)
        {
            string fullArchitypePath = $"{architypePath}/{parameter}.bp";
            IEntity e = EntityFactory.GetEntity(parameter);
            retValue.AddRange(e.GetComponents());
        }
        return retValue;
    }
}
