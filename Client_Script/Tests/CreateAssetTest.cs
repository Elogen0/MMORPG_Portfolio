using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Kame.Define;
using Kame.Utils;
using UnityEditor;
using UnityEngine;

public class CreateAssetTest : ScriptableObject
{
    public AudioClip audio;
#if UNITY_EDITOR
    [MenuItem("Tools/Test")]
    public static void Test()
    {
        GameUtil.Editors.CreateAsset<CreateAssetTest>("Assets/ScriptableObjects/Test/", "TestAsset.asset");
    }
    
    public static CreateAssetTest Get()
    {
        return GameUtil.Editors.CreateAsset<CreateAssetTest>(AssetPath.ScriptableObject + "Test", "TestAsset.asset");
    }
#endif


}
