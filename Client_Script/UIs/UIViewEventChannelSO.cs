using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Int Event Channel", fileName = "UIViewEventChannel")]
public class UIViewEventChannelSO : StringEventChannelSO
{
    private Dictionary<string, UI_View> viewDic = new Dictionary<string, UI_View>();
    public Dictionary<string, UI_View> ViewDic => viewDic; 

    public void Show(string viewName)
    {
        UI_ViewNavigation.Instance.Show(viewName);
    }

    public void Home()
    {
        UI_ViewNavigation.Instance.Home();
    }

    public void Back()
    {
        UI_ViewNavigation.Instance.Back();
    }
    
    public void RegisterView(string viewName, UI_View view)
    {
        viewDic[viewName] = view;
    }

    public void DeRegisterView(string viewName, UI_View view)
    {
        if (viewDic.ContainsKey(viewName))
        {
            viewDic.Remove(viewName);
        }
    }

    public UI_View GetView(string viewName)
    {
        if (viewDic.ContainsKey(viewName))
        {
            return viewDic[viewName];
        }
        return null;
    }

    // protected const string GroupName = "Helper";
    // protected const string Address = AssetPath.ScriptableObject + GroupName;
    // protected const string Name = "UIEventChannel";
    
//     
// #if UNITY_EDITOR
//     [MenuItem("Tools/Helper/UIEvent")]
//     public static void CreateScriptableObject()
//     {
//          UIViewEventChannel obj = GameUtil.Editors.LoadAsset<UIViewEventChannel>("Assets/Resources/ScriptableObjects/Helper/", "UIEventChannel.asset");
//         AddressableUtil.RegisterAsset(obj, GroupName);
//         AddressableUtil.SetLabel(obj, GroupName);
//          EditorUtility.SetDirty(obj);
//     }
// #endif
}
