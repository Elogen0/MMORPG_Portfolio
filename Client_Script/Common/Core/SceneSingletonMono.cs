using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneSingletonMono<T> : MonoBehaviour where T : SceneSingletonMono<T>
{
    /// <summary>
    /// The static reference to the instance
    /// </summary>
    public static T Instance { get; protected set; }

    /// <summary>
    /// Gets whether an instance of this singleton exists
    /// </summary>
    public static bool instanceExists => Instance;

    /// <summary>
    /// Awake method to associate singleton with instance
    /// </summary>
    protected virtual void Awake()
    {
        if (instanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
    
    public static T GetOrCreateInstance()
    {
        if (Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));

            if (Instance == null)
            {
                GameObject _newGameObject = new GameObject(typeof(T).Name, typeof(T));
                Instance = _newGameObject.GetComponent<T>();
            }
        }
        return Instance;
    }

    /// <summary>
    /// OnDestroy method to clear singleton association
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
