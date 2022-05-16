using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame
{
    /// <summary>
    /// Singleton Template Class.
    /// </summary>
    public abstract class SingletonMono<T> : MonoBehaviour where T : Component
    {
        private static object _lock = new object();
        public static bool shuttingDown = false;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (shuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (_instance == null)
                        {
                            GameObject newGameObject = new GameObject(typeof(T).Name, typeof(T));
                            _instance = newGameObject.GetComponent<T>();
                            SetDontDestroy(newGameObject.transform);
                        }
                    }
                }
                return _instance;
            }
        }

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
            
            SetDontDestroy(transform);
        }

        private static void SetDontDestroy(Transform transform)
        {
            if (Application.isPlaying == true)
            {
                //최상위에만 DontDestory가 붙어있어야함
                //Child에 DontDestory가 붙어있으면 에러가뜸 
                if (transform.parent != null && transform.root != null) 
                {
                    DontDestroyOnLoad(transform.root.gameObject);
                }
                else
                {
                    DontDestroyOnLoad(transform.gameObject);
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (_instance)
            {
                shuttingDown = true;
            }
        }

        private void OnDestroy()
        {
            if (_instance && _instance == this)
            {
                shuttingDown = true;
            }
        }
    }
}