using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using Kame.Core.Job;
using Kame.Utilcene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityObject = UnityEngine.Object;

namespace Kame
{
    public static class AddressableLoader
    {
        #region Load
        public static void LoadAssetAsync<T>(object key, Action<T> complete) where T : UnityObject
        {
            Addressables.LoadAssetAsync<T>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    complete.Invoke(handle.Result);
                else
                {
                    Release(handle);
                    complete.Invoke(null);
                }
            };
        }

        public static AssetRequest<T> LoadAsset<T>(object key) where T : UnityObject
        {
            AssetRequest<T> request = new AssetRequest<T>();
            Addressables.LoadAssetAsync<T>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    request.Handle = handle;
                    request.Result = handle.Result;
                    request.isFinished = true;
                }
                else
                {
                    Release(handle);
                    request.Result = null;
                    request.isFinished = true;
                }
            };
            return request;
        }
        #endregion

        #region Instantiate Async
        public static void InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> completed = null)
        {
            if (location == null)
            {
                completed?.Invoke(null);
                return;
            }
            Addressables.InstantiateAsync(location, position, rotation, parent).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(handle);
                    completed?.Invoke(null);
                    return;
                }

                completed?.Invoke(handle.Result);
            };
        }

        public static void InstantiateAsync(string path, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> completed = null)
        {
            GetLocationAsync(path, location =>
            {
                InstantiateAsync(location, position, rotation, parent, completed);
            });
        }
        
        public static void InstantiateAsync(AssetReference reference, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> completed = null)
        {
            GetLocationAsync(reference, location =>
            {
                InstantiateAsync(location, position, rotation, parent, completed);
            });
        }

        public static void InstantiateAsync(IResourceLocation location, Transform parent = null, Action<GameObject> completed = null)
        {
            if (location == null)
            {
                completed?.Invoke(null);
                return;
            }
            Addressables.InstantiateAsync(location, parent).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(handle);
                    completed?.Invoke(null);
                    return;
                }

                completed?.Invoke(handle.Result);
            }; 
        }

        public static void InstantiateAsync(AssetReference reference, Transform parent = null, Action<GameObject> completed = null)
        {
            GetLocationAsync(reference, location =>
            {
                InstantiateAsync(location, parent, completed);
            });
        }
        public static void InstantiateAsync(string path, Transform parent = null, Action<GameObject> completed = null)
        {
            GetLocationAsync(path, location =>
            {
                InstantiateAsync(location, parent, completed);   
            });
        }
        #endregion

        #region Instantiate
        public static AssetRequest<GameObject> Instantiate(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(location, position, rotation, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> Instantiate(string path, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(path, position, rotation, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> Instantiate(AssetReference reference, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(reference, position, rotation, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> Instantiate(IResourceLocation location, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(location, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> Instantiate(string path, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(path, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> Instantiate(AssetReference reference, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsync(reference, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }
        #endregion
        
        #region Pooling
        public static AssetRequest<GameObject> InstantiatePooling(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsyncPooling(location, position, rotation, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> InstantiatePooling(string path, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            GetLocationAsync(path, location =>
            {
                InstantiateAsyncPooling(location, position, rotation, parent, go =>
                {
                    request.Result = go;
                    request.isFinished = true;
                });    
            });
            return request;
        }
        
        public static AssetRequest<GameObject> InstantiatePooling(AssetReference reference, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            GetLocationAsync(reference, location =>
            {
                InstantiateAsyncPooling(location, position, rotation, parent, go =>
                {
                    request.Result = go;
                    request.isFinished = true;
                });
            });
            return request;
        }
        
        public static AssetRequest<GameObject> InstantiatePooling(IResourceLocation location, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            InstantiateAsyncPooling(location, parent, go =>
            {
                request.Result = go;
                request.isFinished = true;
            });
            return request;
        }

        public static AssetRequest<GameObject> InstantiatePooling(string path, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            GetLocationAsync(path, location =>
            {
                InstantiateAsyncPooling(location, parent, go =>
                {
                    request.Result = go;
                    request.isFinished = true;
                });
            });
            return request;
        }
        
        public static AssetRequest<GameObject> InstantiatePooling(AssetReference reference, Transform parent = null)
        {
            AssetRequest<GameObject> request = new AssetRequest<GameObject>();
            GetLocationAsync(reference, location =>
            {
                InstantiateAsyncPooling(location, parent, go =>
                {
                    request.Result = go;
                    request.isFinished = true;
                });
            });
            return request;
        }
        
        private static void InstantiateAsyncPooling(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> complete = null)
        {
            if (location == null)
            {
                complete?.Invoke(null);
                return;
            }
            AddressablePoolManager.Instance.GetAsync(location, position, rotation, parent, complete);
        }
        
        private static void InstantiateAsyncPooling(IResourceLocation location, Transform parent = null, Action<GameObject> complete = null)
        {
            if (location == null)
            {
                complete?.Invoke(null);
                return;
            }
            AddressablePoolManager.Instance.GetAsync(location, parent, complete);
        }
        #endregion Pooling

        #region Location
        public static void GetLocationAsync(object key, Action<IResourceLocation> complete)
        {
            GetLocationsAsync(key, (locations) =>
            {
                if (locations == null)
                {
                    complete(null);
                    return;
                }
                complete(locations[0]);
            });
        }
        
        public static void GetLocationsAsync(object key, Action<IList<IResourceLocation>> complete)
        {
            try
            {
                Addressables.LoadResourceLocationsAsync(key).Completed += handle =>
                {
                    if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count <= 0)
                    {
                        Addressables.Release(handle);
                        Debug.LogWarning($"Can not Found Asset : {key}");
                        complete(null);
                        return;
                    }
                
                    complete(handle.Result);
                };
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                complete(null);
            }
        }
        #endregion
        
        #region Clear
        public static void Release(object obj)
        {
            if (obj != null)
                Addressables.Release(obj);
        }

        public static void Release(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        
        public static void ReleaseInstance(GameObject go, float time = 0f)
        {
            if (!go)
                return;
            if (time != 0)
            {
                IJob job = ResourceTimer.Instance.PushAfter((int)(time * 1000), () => { ReleaseInstance(go); });
                return;
            }
            if (!AddressablePoolManager.shuttingDown)
                AddressablePoolManager.Instance.Release(go);
        }

        public static void Clear()
        {
            if (!AddressablePoolManager.shuttingDown)
                AddressablePoolManager.Instance.Clear();
            AddressableUtil.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        #endregion

    }

    public class AssetRequest<T>
    {
        public AsyncOperationHandle<T> Handle { get; set; }
        public T Result { get; set; }
        public bool isFinished = false;
        
        public IEnumerator Wait() => new WaitUntil(() => isFinished);
    }
}
