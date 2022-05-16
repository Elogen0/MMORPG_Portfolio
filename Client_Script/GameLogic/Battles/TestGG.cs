using System;
using System.Collections;
using Kame;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestGG : MonoBehaviour
{
    public AssetReference cube;
    private GameObject cubeObj;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(CoInstantiate());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            AddressableLoader.ReleaseInstance(cubeObj);
        }
    }

    IEnumerator CoInstantiate()
    {
        var request = AddressableLoader.Instantiate(cube);
        yield return request.Wait();
        cubeObj = request.Result;
    }
}
