using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton<T> : MonoBehaviour where T : Component
{
    private static object _lock = new object();
    private static T _instance;

    public static T Instance => Init();
    public static bool InstanceExist => _instance != null;
    protected virtual void Awake()
    {
        lock (_lock)
        {
            var objects = FindObjectsOfType(GetType());
            if (objects.Length > 1 && _instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }
    }

    private static T Init()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = (T) FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    GameObject newGameObject = new GameObject(typeof(T).Name, typeof(T));
                    _instance = newGameObject.GetComponent<T>();
                }
            }
        }

        return _instance;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}