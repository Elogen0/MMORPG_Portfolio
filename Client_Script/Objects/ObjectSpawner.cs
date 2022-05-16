using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;

public class ObjectSpawner : SingletonMono<ObjectSpawner>
{
    public void SpawnAsync(GameObject caller, string path, Vector3 position, Quaternion rotation, Transform parent = null, object data = null, Action<GameObject> complete = null)
    {
        StartCoroutine(CoSpawn(caller, path, position, rotation, parent, data, complete));
    }
    
    public void SpawnAsync(GameObject caller, string path, Transform parent = null, object data = null, Action<GameObject> complete = null)
    {
        StartCoroutine(CoSpawn(caller, path, parent, data, complete));
    }

    public void StopSpawn()
    {
        StopAllCoroutines();
    }

    IEnumerator CoSpawn(GameObject caller, string path , Vector3 position, Quaternion rotation, Transform parent, object data, Action<GameObject> complete)
    {
        var request = AddressableLoader.Instantiate(path, position, rotation, parent);
        yield return request.Wait();
        var go = request.Result;
        complete?.Invoke(go);
        if (go.TryGetComponent(out Spawnee spawnee))
        {
            yield return spawnee.AfterSpawnAction(caller, data);
        }
    }
    
    IEnumerator CoSpawn(GameObject caller, string path, Transform parent, object data, Action<GameObject> complete)
    {
        var request = AddressableLoader.Instantiate(path, parent);
        yield return request.Wait();
        var go = request.Result;
        complete?.Invoke(go);
        if (go.TryGetComponent(out Spawnee spawnee))
        {
            yield return spawnee.AfterSpawnAction(caller, data);
        }
    }
}
