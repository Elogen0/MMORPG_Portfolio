using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Utilcene;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Initializer : MonoBehaviour
{
    [SerializeField] string startSceneName;
    private void Start()
    {
        AddressableUtil.InitializeAsync();
        Debug.Log($"StartScene : {startSceneName}");
        // GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadScene(startSceneName);
        SceneLoader.Instance.LoadScene(startSceneName);
    }
}
