using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MakePngUniform
{
    [MenuItem("Reclamation/Make PNGs Uniform")]
    public static void MakePngExtensionsUniform()
    {
        foreach (var file in Directory.EnumerateFiles("Assets/", "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".PNG"))
            {
                string error = AssetDatabase.RenameAsset(file, Path.GetFileName(file).Replace(".PNG", ".png"));
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError(error);
                    break;
                }
            }
        }
    }
}
