using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FullBuild
{
    [MenuItem("Reclamation/Full Build")]
    public static void DoFullBuild()
    {
        string bpFolder = "Blueprints";
        string levelData = "LevelData";

        string finalBuildPath = "/BitDraft/ReclamationBuild/";

        if (Directory.Exists(finalBuildPath))
            Directory.Delete(finalBuildPath, true);
        Directory.CreateDirectory(finalBuildPath);

        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            locationPathName = finalBuildPath + "Relamation.exe",
            scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
            target = EditorUserBuildSettings.activeBuildTarget,
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup
        });

        CopyFilesRecursively(bpFolder, finalBuildPath + bpFolder + "/");
        CopyFilesRecursively(levelData, finalBuildPath + levelData + "/");
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}
