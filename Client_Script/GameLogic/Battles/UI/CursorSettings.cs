using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using Kame.Utilcene;
using Kame.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CursorSettings : ScriptableObject
{
    public CursorMapping[] cursorMappings;
    
    protected const string GroupName = "Helper";
    protected const string Address = ResourcePath.ScriptableObject + GroupName;
    protected const string Name = "CursorSettings";
    public static CursorSettings Get()
    {
        //return ResourceLoader.Load<UIEventChannel>(Directory + "/" + Name);
        return Resources.Load<CursorSettings>($"{Address}/{Name}");
    }
    
#if UNITY_EDITOR
    [MenuItem("Tools/Helper/CursorSettings")]
    public static void CreateScriptableObject()
    {
        // CursorSettings obj = GameUtil.Editors.LoadAsset<CursorSettings>(Address, Name);
        // AddressableUtil.RegisterAsset(obj, GroupName);
        // AddressableUtil.SetLabel(obj, GroupName);
        // EditorUtility.SetDirty(obj);
    }
#endif
}
