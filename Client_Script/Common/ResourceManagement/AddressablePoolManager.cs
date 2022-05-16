using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Kame.Utilcene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace Kame
{
    public class AddressablePoolManager : SingletonMono<AddressablePoolManager>
    {
        #region Pool
        class Pool
        {
            public IResourceLocation Location { get; private set; }
            public int key;
            public List<GameObject> _cacheList = new List<GameObject>();
            public Stack<GameObject> _usableStack = new Stack<GameObject>();
            public void Init(IResourceLocation location, int count = 5)
            {
                Location = location;
                key = location.Hash(typeof(GameObject));
                for (int i = 0; i < count; i++)
                {
                    Create(null, Release);
                }
            }
            
            public void GetAsync(Transform parent, Vector3 position, Quaternion rotation, Action<GameObject> completed)
            {
                if (_usableStack.Count > 0)
                {
                    GameObject go = _usableStack.Pop();
                    if (go.transform.parent != parent)
                        go.transform.SetParent(parent);
                    go.transform.SetPositionAndRotation(position, rotation);
                    go.SetActive(true);
                    completed?.Invoke(go);
                }
                else
                {
                    for (int i = 0; i < _cacheList.Count; i++)
                    {
                        Create(parent, (go) =>
                        {
                            go.transform.SetPositionAndRotation(position, rotation);
                            go.SetActive(true);
                            completed?.Invoke(go);
                        });    
                    }
                }
            }
            
            public void GetAsync(Transform parent, Action<GameObject> completed)
            {
                if (_usableStack.Count > 0)
                {
                    GameObject go = _usableStack.Pop();
                    if (go.transform.parent != parent)
                        go.transform.SetParent(parent);
                    go.SetActive(true);
                    completed?.Invoke(go);
                }
                else
                {
                    Create(parent, (go) =>
                    {
                        go.SetActive(true);
                        completed?.Invoke(go);
                    });
                }
            }
            
            public void Release(GameObject go)
            {
                if (!go)
                    return;
                go.SetActive(false);
                go.transform.SetParent(null);
                _usableStack.Push(go);
            }
            
            public void Clear()
            {
                foreach (var go in _cacheList)
                {
                    if (go)
                    {
                        DestroyObject(go.gameObject);
                    }
                }
                _cacheList.Clear();
                _usableStack.Clear();
            }
            
            public static void DestroyObject(GameObject obj)
            {
                if (!obj)
                    return;
                if (!Addressables.ReleaseInstance(obj))
                    Object.Destroy(obj);
                obj = null;
            }
            
            private void Create(Transform parent, Action<GameObject> completed)
            {
                Addressables.InstantiateAsync(Location, parent).Completed += handle =>
                {
                    GameObject go = handle.Result;
                    var poolable = go.GetOrAddComponent<Poolable>();
                    poolable.key = key;
                    //go.name = Location.Hash(typeof(GameObject)).ToString();
                    _cacheList.Add(go);
                    completed(go);
                };
            }
        }
        #endregion
        
        private Dictionary<int, Pool> _pools = new Dictionary<int, Pool>();

        public void GetAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> completed = null)
        {
            int poolKey = location.Hash(typeof(GameObject));
            
            if (!_pools.ContainsKey(poolKey))
            {
                CreatePool(location);
            }
            _pools[poolKey].GetAsync(parent, position, rotation, completed);
        }
        
        public void GetAsync(IResourceLocation location, Transform parent = null, Action<GameObject> completed = null)
        {
            int poolKey = location.Hash(typeof(GameObject));
            
            if (!_pools.ContainsKey(poolKey))
            {
                CreatePool(location);
            }
            _pools[poolKey].GetAsync(parent, completed);
        }
        
        public void Release(GameObject go)
        {
            if (!go)
                return;

            if (go.TryGetComponent(out Poolable poolable) )
            {
                if ( _pools.ContainsKey(poolable.key))
                    _pools[poolable.key].Release(go);
                return;
            }
            if (!Addressables.ReleaseInstance(go))
            {
                Object.Destroy(go);
            }
            go = null;
        }
        
        public IResourceLocation GetLocation(int key)
        {
            if (!_pools.ContainsKey(key))
                return null;
            return _pools[key].Location;
        }

        public bool ContainsKey(int key)
        {
            return _pools.ContainsKey(key);
        }

        public void Clear()
        {
            foreach (var pool in _pools)
            {
                pool.Value.Clear();
            }
            _pools.Clear();
        }

        public void Clear(IResourceLocation location)
        {
            if (location == null)
                return;
            
            int poolKey = location.Hash(typeof(GameObject));
            if (!_pools.ContainsKey(poolKey)) return;

            _pools[poolKey].Clear();
            _pools.Remove(poolKey);
        }
        
        private void CreatePool(IResourceLocation location, int poolSize = 1)
        {
            int poolKey = location.Hash(typeof(GameObject));

            Pool pool; 
            if (!_pools.ContainsKey(poolKey))
            {
                pool = new Pool();
                _pools.Add(poolKey, pool);
            }
            else
            {
                pool = _pools[poolKey];
            }

            if (pool._cacheList.Count < poolSize)
            {
                pool.Init(location, poolSize - pool._cacheList.Count);
            }
        }
    }
    
    
}

