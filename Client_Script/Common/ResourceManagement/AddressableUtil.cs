using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

namespace Kame.Utilcene
{
    public class AddressableUtil
    {
        private AddressableUtil() { }

        public static void InitializeAsync()
        {
            var async = Addressables.InitializeAsync();
            async.Completed += (op) =>
            {
                Addressables.Release(async);
            };
        }

        #region Download

        public static void DownLoadAssetAsync(object key, Action completed = null)
        {
            Addressables.GetDownloadSizeAsync(key).Completed += (opSize) =>
            {
                if (opSize.Status == AsyncOperationStatus.Succeeded && opSize.Result > 0)
                {
                    Addressables.DownloadDependenciesAsync(key, true).Completed += (opDownload) =>
                    {
                        switch (opDownload.Status)
                        {
                            case AsyncOperationStatus.Succeeded:
                                completed?.Invoke();
                                break;
                            case AsyncOperationStatus.Failed:
                                Debug.Log($"Can not Download[{key}]");
                                break;
                        }
                    };
                }
                //result값이 0이면 다운로드가 완료된 상태
                else if (opSize.Result == 0)
                {
                    completed?.Invoke();
                }
            };
        }

        IEnumerator UpdateCatalogs()
        {
            List<string> catalogsToUpdate = new List<string>();
            AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += (handle) => { catalogsToUpdate.AddRange(handle.Result); };
            yield return checkForUpdateHandle;
            if (catalogsToUpdate.Count > 0)
            {
                AsyncOperationHandle<List<IResourceLocator>> updateHandle =
                    Addressables.UpdateCatalogs(catalogsToUpdate);
                yield return updateHandle;
            }
        }

        public static void DeleteDownloadedAsset(string address)
        {
            Addressables.ClearDependencyCacheAsync(address);
        }

        public static void DeleteAllDownloaded()
        {
            Caching.ClearCache();
        }

        #endregion

        #region Load

        // ReSharper disable Unity.PerformanceAnalysis
        public static async Task<T> Load<T>(object key) where T : UnityEngine.Object
        {
            var task = await Addressables.LoadAssetAsync<T>(key).Task;
            return task;
        }

        public static T LoadSync<T>(object key) where T : UnityEngine.Object
        {
            var task = Load<T>(key);
            task.Start();
            return task.GetAwaiter().GetResult();
        }

        public static bool TryLoad<T>(object key, out T result) where T : UnityEngine.Object
        {
            var task = Load<T>(key);
            result = task.Result;
            if (result == default(GameObject))
            {
                return false;
            }
            return true;
        }

        
        public static async void LoadAsync<T>(object key, Action<T, int> completed = null) where T : UnityEngine.Object
        {
            var locations = await Addressables.LoadResourceLocationsAsync(key).Task;
            //await Task.Yield();
            if (locations.Count < 0)
            {
                Debug.Log($"LoadAsync Failed.");
                return;
            }

            foreach (var location in locations)
            {
                Addressables.LoadAssetAsync<T>(location).Completed += (handle) =>
                {
                    //Debug.Log("Load: " + location.PrimaryKey.GetHashCode());
                    completed?.Invoke(handle.Result, locations.Count);
                };
            }
        }

        public static async Task<IList<IResourceLocation>> GetLocations(object path)
        {
            var task = await Addressables.LoadResourceLocationsAsync(path).Task;
            return task;
        }
        
        public static string GetAddress(object key)
        {
            return GetLocations(key).Result.First()?.PrimaryKey;
        }

        public static bool AddressableResourceExists<T>(object key) where T : Object 
        {
            foreach (var l in Addressables.ResourceLocators) 
            {
                if (l.Locate(key, typeof(T), out IList<IResourceLocation> locs))
                    return true;
            }
            return false;
        }

        #endregion

        #region SpriteAtlas

        private static Dictionary<int, Sprite> _spriteCache = new Dictionary<int, Sprite>();

        public static void LoadSpriteInAtlas(string atlasAddress, string spriteName, Action<Sprite> completed = null)
        {
            string atlasSpriteAddress = $"{atlasAddress}[{spriteName}]";
            Addressables.LoadAssetAsync<Sprite>(atlasSpriteAddress).Completed += (handle) =>
            {
                OnSpriteLoaded(handle, completed);
            };
        }

        public static void LoadSpriteInAtlas(AssetReferenceAtlasedSprite atlasedSprite, Action<Sprite> completed = null)
        {
            atlasedSprite.LoadAssetAsync().Completed += (handle) => { OnSpriteLoaded(handle, completed); };
        }

        private static void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle, Action<Sprite> completed)
        {
            switch (handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    _spriteCache.Add(handle.Result.name.GetHashCode(), handle.Result);
                    completed?.Invoke(handle.Result);
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"Atlas Sprite load Failed. {handle.DebugName}");
                    break;
            }
        }

        #endregion

        
        #region Instantiate
        
        public static async Task<GameObject> Instantiate(object key, Transform parent = null) 
        {
            var task = await Addressables.InstantiateAsync(key, parent).Task;
            return task;
        }
        
        public static async Task<GameObject> Instantiate(IResourceLocation location, Transform parent = null) 
        {
            var task = await Addressables.InstantiateAsync(location, parent).Task;
            return task;
        }

        public static void InstantiateAsync(object key, Transform parent = null, Action<GameObject> completed = null)
        {
            Addressables.InstantiateAsync(key, parent).Completed += (handle) =>
            {
                OnInstantiateDone(handle, completed);
            };
        }

        public static void InstantiateAsync(object key, Vector3 position, Quaternion rotation,
            Transform parent = null, Action<GameObject> completed = null)
        {
            Addressables.InstantiateAsync(key, position, rotation, parent).Completed += (handle) =>
            {
                OnInstantiateDone(handle, completed);
            };
        }

        public static void InstantiateAsyncWith(AssetLabelReference label, Action<GameObject> completed = null)
        {
            Addressables.LoadResourceLocationsAsync(label).Completed += (op) =>
            {
                foreach (var location in op.Result)
                {
                    Addressables.InstantiateAsync(location).Completed += (handle) =>
                    {
                        OnInstantiateDone(handle, completed);
                    };
                }
            };
        }

        private static void OnInstantiateDone(AsyncOperationHandle<GameObject> handle, Action<GameObject> completed)
        {
            switch (handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                {
                    completed?.Invoke(handle.Result);
                }
                    break;
                case AsyncOperationStatus.Failed:
                {
                    Debug.Log($"Instantiate Failed. {handle.DebugName}");
                }
                    break;
                default:
                    break;
            }
        }

        #endregion

        public static void Destroy(GameObject obj)
        {
            if (!obj)
                return;
            if (!Addressables.ReleaseInstance(obj))
            {
                Object.Destroy(obj);
            }
            obj = null;
        }

        public static void Clear()
        {
            foreach (var pair in _spriteCache)
            {
                Addressables.Release(pair.Value);
            }
            _spriteCache.Clear();

            //1.여기까지
            //AssetBundle.UnloadAllAssetBundles(true);
        }
#if UNITY_EDITOR
        public static void RegisterAsset(Object obj, string groupName)
        {
            if (!obj)
                return;
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings)
            {
                var group = settings.FindGroup(groupName);
                if (!group)
                    group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema),
                        typeof(BundledAssetGroupSchema));
                var assetPath = AssetDatabase.GetAssetPath(obj);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                
                
                var e = settings.CreateOrMoveEntry(guid, group, false, false);
                var entriesAdded = new List<AddressableAssetEntry> {e};
                group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            }
        }

        public static void SetLabel(Object obj, string label)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                var e = settings.FindAssetEntry(guid);
                e.SetLabel(label, true, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, e, true, false);
            }
        }

        public static bool BuildPlayerContents()
        {
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            return result.Error == string.Empty;
        }
#endif
    }
}