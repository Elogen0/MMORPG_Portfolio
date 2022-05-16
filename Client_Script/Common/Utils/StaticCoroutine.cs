using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticCoroutine
{
    private class SubClass : MonoBehaviour { }

    private static SubClass subInstance;

    private static void Init()
    {
        if (!subInstance)
        {
            GameObject gameObject = new GameObject("StaticCoroutine", typeof(SubClass));
            subInstance = gameObject.GetComponent<SubClass>();
        }
    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        Init();
        return subInstance.StartCoroutine(coroutine);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (!subInstance)
            return;
        if (coroutine != null)
        {
            subInstance.StopCoroutine(coroutine);
        }
    }

    public static void StopAll()
    {
        if (!subInstance)
            return;
        subInstance.StopAllCoroutines();
    }
}
