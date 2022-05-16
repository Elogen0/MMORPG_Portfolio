using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Core.Job;
using UnityEngine;

public class ResourceLoader
{
    public static T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = ObjectPoolManager.GetPrefab(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public static GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return ObjectPoolManager.Get(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public static void Destroy(GameObject go, float time = 0)
    {
        if (go == null)
            return;

        if (time != 0)
        {
            if (!ResourceTimer.shuttingDown)
            {
                IJob job = ResourceTimer.Instance.PushAfter((int)(time * 1000), () => { Destroy(go); });
            }
            return;
        }

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            ObjectPoolManager.Release(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
