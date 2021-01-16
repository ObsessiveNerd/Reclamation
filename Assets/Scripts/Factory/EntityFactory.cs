using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class EntityFactory
{
    private static string m_BluePrintPath = "Assets/Blueprints";
    public static IEntity CreateEntity(string blueprintName)
    {
        Actor a = new Actor("<empty>");
        string path = $"{m_BluePrintPath}/{blueprintName}.bp";
        using (var stream = new StreamReader(path))
        {
            string header = stream.ReadLine();
            int firstIndex = header.IndexOf('<');
            int lastIndex = header.IndexOf('>');
            IEntity archiType = null;
            string architype = header.Substring(0, firstIndex > 0 ? firstIndex : header.IndexOf('('));
            string name = architype;

            if (firstIndex > 0 && lastIndex > 0)
            {
                int nameStart = firstIndex + 1;
                int nameLength = lastIndex - firstIndex - 1;
                name = header.Substring(nameStart, nameLength);

                archiType = CreateEntity($"Architype/{architype}");
            }

            a = new Actor(name);

            string line;
            while((line = stream.ReadLine()) != null && line != ")")
            {
                string[] componentToValues = line.Split(':');
                string dtoComponentName = $"DTO_{componentToValues[0].Replace("\t", string.Empty)}";
                Type componentType = Type.GetType(dtoComponentName);
                if (componentType != null)
                {
                    IDataTransferComponent dtc = (IDataTransferComponent)Activator.CreateInstance(componentType);
                    if (componentToValues.Length == 1)
                        dtc.CreateComponent("");
                    else
                    {
                        string data = componentToValues[1];
                        if(!data.StartsWith("\"") && !data.EndsWith("\""))
                            data = componentToValues[1].Replace(" ", string.Empty);
                        dtc.CreateComponent(data);
                    }
                    a.AddComponent(dtc.Component);
                }
                else
                    Debug.LogError($"Couldn't find componentType for {dtoComponentName}");
            }
            if (archiType != null)
                a.AddComponentRange(archiType.GetComponents());
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

    public static string GetEntityNameFromBlueprintFormatting(string bpFormatting)
    {
        int start = bpFormatting.IndexOf('<') + 1;
        int length = bpFormatting.IndexOf('>') - start;
        return bpFormatting.Substring(start, length);
    }
}
