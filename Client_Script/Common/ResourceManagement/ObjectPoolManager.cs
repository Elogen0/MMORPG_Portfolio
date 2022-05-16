using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Utilcene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Kame
{
    public class ObjectPoolManager
    {
        #region Pool
        class Pool
        {
            public GameObject Prefab { get; private set; }
            public List<Poolable> _cacheList = new List<Poolable>();
            public Stack<Poolable> _stack = new Stack<Poolable>();
            public void Init(GameObject prefab, int count = 5)
            {
                Prefab = prefab;
                for (int i = 0; i < count; i++)
                {
                    Poolable poolable = Create();
                    Release(Create());                    
                }
            }

            private Poolable Create(Transform parent = null)
            {
                GameObject go = Object.Instantiate(Prefab, parent);
                go.name = Prefab.name;
                Poolable poolable = go.GetOrAddComponent<Poolable>();
                _cacheList.Add(poolable);
                return poolable;
            }

            public void Release(Poolable poolable)
            {
                if (!poolable)
                    return;
                poolable.gameObject.SetActive(false);
            }

            public Poolable Get(Transform parent)
            {
                Poolable poolable;

                if (_stack.Count > 0)
                {
                    poolable = _stack.Pop();
                    if (parent && poolable.transform.parent != parent)
                        poolable.transform.parent = parent;
                }
                else
                {
                    poolable = Create(parent);
                }
                
                poolable.gameObject.SetActive(true);

                return poolable;
            }

            public void Clear()
            {
                foreach (var poolable in _cacheList)
                {
                    if (poolable)
                    {
                        DestroyObj(poolable.gameObject);
                    }
                }
                _cacheList.Clear();
                _stack.Clear();
            }
            
            public void DestroyObj(GameObject obj)
            {
                if (!obj)
                    return;
                Object.Destroy(obj);
                obj = null;
            }
        }
        #endregion
        
        private static Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        public static void CreatePool(GameObject prefab, int poolSize = 5)
        {
            if (!prefab)
                return;
            string poolKey = prefab.name;

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
                pool.Init(prefab, poolSize - pool._cacheList.Count);
            }
        }

        public static Poolable Get(GameObject prefab, Transform parent = null)
        {
            if (!prefab)
                return null;
            
            string poolKey = prefab.name;
            
            if (!_pools.ContainsKey(poolKey))
            {
                CreatePool(prefab);
            }
            Poolable pool = _pools[poolKey].Get(parent);
            return pool;
        }
        
        public static GameObject GetPrefab(string name)
        {
            if (!_pools.ContainsKey(name))
                return null;
            return _pools[name].Prefab;
        }

        public static void Release(Poolable poolable)
        {
            if (!poolable)
                return;
            string poolKey = poolable.gameObject.name;
            if (!_pools.ContainsKey(poolKey))
            {
                Object.Destroy(poolable.gameObject);
                return;
            }
            _pools[poolKey].Release(poolable);
        }

        public static void Clear()
        {
            foreach (var pool in _pools)
            {
                pool.Value.Clear();
            }
            _pools.Clear();
        }

        public static void Clear(Poolable poolable)
        {
            if (!poolable)
                return;
            
            string poolKey = poolable.gameObject.name;
            if (!_pools.ContainsKey(poolKey)) return;

            _pools[poolKey].Clear();
            _pools.Remove(poolKey);
        }
    }
}