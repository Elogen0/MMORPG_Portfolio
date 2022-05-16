using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kame.Utils
{
    public static class GameUtil
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
    		if (component == null)
                component = go.AddComponent<T>();
            return component;
    	}
    
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            if (transform == null)
                return null;
            
            return transform.gameObject;
        }
    
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;
    
            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
    		}
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }
    
            return null;
        }


        public static class Editors
        {
#if UNITY_EDITOR
            public static T CreateAsset<T>(string directory, string name, bool create = true) where T : ScriptableObject
            {
                char[] charsToTrim = {'/', ' '};
                directory = directory.Trim(charsToTrim);
                if (create)
                    CreateFolder(directory);

                T instance = null;
                instance = AssetDatabase.LoadAssetAtPath<T>($"{directory}/{name}");
                if (instance)
                    return instance;

                if (create)
                {
                    instance = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(instance, $"{directory}/{name}");
                    Debug.Log($"Create Asset : {directory}/{name}");
                }
                else
                {
                    Debug.LogError($"Cannot Found Asset : {directory}/{name}");
                }

                return instance;
            }

            public static void CreateFolder(string directory)
            {
                string[] splited = directory.Split('/');
                if (splited.Length <= 0)
                    return;

                string prevDirectory = splited[0];
                for (int i = 1; i < splited.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder($"{prevDirectory}/{splited[i]}"))
                    {
                        AssetDatabase.CreateFolder(prevDirectory, splited[i]);
                    }

                    prevDirectory = $"{prevDirectory}/{splited[i]}";
                }
            }

            private static bool TryGetActiveFolderPath(out string path)
            {
                var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath",
                    BindingFlags.Static | BindingFlags.NonPublic);

                object[] args = new object[] {null};
                bool found = (bool) _tryGetActiveFolderPath.Invoke(null, args);
                path = (string) args[0];

                return found;
            }

            public static bool IsExistAsset(UnityEngine.Object asset) =>
                AssetDatabase.GetAssetPath(asset) != string.Empty;

            public static string GetSelectedPathOrFallback()
            {
                string path = "Assets";

                foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object),
                    SelectionMode.Assets))
                {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        path = Path.GetDirectoryName(path);
                        break;
                    }
                }

                return path;
            }
#endif
        }
    }
}
