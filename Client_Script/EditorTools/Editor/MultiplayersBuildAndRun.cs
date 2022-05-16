using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Content;
using UnityEngine;

public class MultiplayersBuildAndRun : MonoBehaviour
{
    [MenuItem("Tools/Run MultiPlayer/1 Player")]
    static void PerformWin64Build1() => PerformWin64Build(1);
    [MenuItem("Tools/Run MultiPlayer/2 Player")]
    static void PerformWin64Build2() => PerformWin64Build(2);
    [MenuItem("Tools/Run MultiPlayer/3 Player")]
    static void PerformWin64Build3() => PerformWin64Build(3);
    [MenuItem("Tools/Run MultiPlayer/4 Player")]
    static void PerformWin64Build4() => PerformWin64Build(4);
    
    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"Addressables Build Failded : {result.Error}");
            return;
        }
        
        for (int i = 0; i < playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "Builds/Win64" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64,
                BuildOptions.AutoRunPlayer| BuildOptions.Development);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
