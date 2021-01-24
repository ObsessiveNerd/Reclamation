using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class EntityFactory
{
    private static string m_BluePrintPath = "Assets/Blueprints";
    public static IEntity CreateEntity(string blueprintName)
    {
        Actor a = new Actor("<empty>");
        string path = $"{m_BluePrintPath}/{blueprintName}.bp";
        if(!File.Exists(path))
            path = $"{SaveSystem.kSaveDataPath}/{World.Instance.Seed}/Blueprints/{blueprintName}.bp"; //todo: need proper seed
        if (!File.Exists(path))
            return null;
        return GetEntity(path);
    }

    public static IEntity GetEntity(string path)
    {
        Actor a = null;
        using (var stream = new StreamReader(path))
        {
            string header = stream.ReadLine();
            int firstIndex = header.IndexOf('<');
            int lastIndex = header.IndexOf('>');

            int nameStart = firstIndex + 1;
            int nameLength = lastIndex - firstIndex - 1;
            string name = header.Substring(nameStart, nameLength);

            a = new Actor(name);

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
        return a;
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

            a = new Actor(name);

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
        return a;
    }

    public static List<IEntity> GetEntitiesFromArray(string data)
    {
        List<IEntity> result = new List<IEntity>();
        data = data.Replace(" ", string.Empty).TrimStart('[').TrimEnd(']');
        string[] parameters = data.Split('&');
        foreach(string parameter in parameters)
        {
            if (string.IsNullOrEmpty(parameter))
                continue;

            string entityName = GetEntityNameFromBlueprintFormatting(parameter);
            result.Add(CreateEntity(entityName));
        }
        return result;
    }

    public static string ConvertEntitiesToStringArray(List<IEntity> entities)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var entity in entities)
            sb.Append($"<{entity.ID}>&");
        return sb.ToString().TrimEnd('&');
    }

    public static string GetEntityNameFromBlueprintFormatting(string bpFormatting)
    {
        int start = bpFormatting.IndexOf('<') + 1;
        int length = bpFormatting.IndexOf('>') - start;
        return bpFormatting.Substring(start, length);
    }

    public static void CreateTemporaryBlueprint(string tempPath, string blueprintName, string data)
    {
        string path = $"{SaveSystem.kSaveDataPath}/{tempPath}/Blueprints/{blueprintName}.bp";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, data);
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
            IEntity e = EntityFactory.GetEntity(fullArchitypePath);
            retValue.AddRange(e.GetComponents());
        }
        return retValue;
    }
}
