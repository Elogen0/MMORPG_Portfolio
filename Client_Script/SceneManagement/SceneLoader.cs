using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using Kame;
using Kame.Define;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMono<SceneLoader>
{
    private SceneInstance _currentScene;
    public bool isSceneActivated = false;
    
    // public void LoadScene(string sceneName, Action OnCompleted = null)
    // {
    //     string scenePath = "Assets/Scenes/" + sceneName + ".unity";
    //
    //     if (isSceneActivated)
    //     {
    //         Addressables.UnloadSceneAsync(_currentScene, true).Completed += (unloadHandle) =>
    //         {
    //             AddressableLoader.Clear();
    //             isSceneActivated = false;
    //             LoadSceneAsync(scenePath, OnCompleted);
    //         };
    //     }
    //     else
    //     {
    //         LoadSceneAsync(scenePath, OnCompleted);
    //     }
    // }
    //
    // private void LoadSceneAsync(string sceneName, Action OnCompleted)
    // {
    //     Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive).Completed += handle =>
    //     {
    //         SceneManager.SetActiveScene(handle.Result.Scene);
    //         _currentScene = handle.Result;
    //         isSceneActivated = true;
    //         OnCompleted?.Invoke();
    //     };
    // }
    
    
    public void LoadScene(string sceneName, Action OnCompleted = null)
    {
        StartCoroutine(CoLoadScene(sceneName, OnCompleted));
    }
    
    public IEnumerator CoLoadScene(string sceneName, Action OnCompleted)
    {
        //Fader.Instance.FadeOut(0.5f);
    
        if (isSceneActivated)
        {
            isSceneActivated = false;
            var unloadHandle = Addressables.UnloadSceneAsync(_currentScene, true);
            while (!unloadHandle.IsDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    
        AddressableLoader.Clear();
    
        var handle = Addressables.LoadSceneAsync(AssetPath.Scene + sceneName + ".unity", LoadSceneMode.Additive, false);
        //yield return handle;
        while (!handle.IsDone)
        {
            // if (loadingPercentText)
            //     loadingText.text = $"{handle.PercentComplete}";
            float percentage = handle.PercentComplete;
            //yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
        
    
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _currentScene = handle.Result;
            yield return handle.Result.ActivateAsync();
            if (SceneManager.SetActiveScene(handle.Result.Scene))
            {
                OnCompleted?.Invoke();
            }
        }
        isSceneActivated = true;
        //Fader.Instance.FadeIn(0.5f);
    }
}
