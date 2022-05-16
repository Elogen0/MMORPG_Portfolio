using UnityEngine;

namespace Kame
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] result = Resources.FindObjectsOfTypeAll<T>();
                    if (result.Length == 0)
                    {
                        Debug.LogError($"SingletonScriptableObject<{typeof(T).ToString()}> : result length is 0");
                        return null;
                    }

                    if (result.Length > 1)
                    {
                        Debug.LogError($"SingletonScriptableObject<{typeof(T).ToString()}> : result length is greater than 1");
                        return null;
                    }

                    _instance = result[0];
                    _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }

                return _instance;
            }
        }
    }
}