using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    

    public static bool IsValid(this GameObject go)
    {
        return go && go.activeSelf;
    }

    public static void RemoveAllChildren(this Transform tr)
    {
        for (int i = tr.childCount - 1; i >= 0; --i)
        {
            AddressableLoader.ReleaseInstance(tr.GetChild(i).gameObject);
        }
    }
    
}
