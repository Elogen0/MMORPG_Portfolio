using System;
using Kame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kame.Abilities
{

    [RequireComponent(typeof(ActionScheduler), typeof(CooldownStore))]
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] AssetReferenceAbility[] abilities;

        public bool SendNetwork { get; set; } = false;
        // private Ability[] _loadedAbilities;
        // private bool[] _isLoaded;
        private Dictionary<string, bool> _isLoaded = new Dictionary<string, bool>();
        private Dictionary<string, Ability> _abilityCache = new Dictionary<string, Ability>();
        private AsyncOperationHandle<Ability>[] _handles;
        // Start is called before the first frame update
        
        private void Start()
        {
            StartCoroutine(CoLoad());
        }

        // Update is called once per frame
        void Update()
        {
            // for (int i = 0; i < _loadedAbilities.Length; i++)
            // {
            //     if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            //     {
            //         if (_isLoaded[i])
            //             _loadedAbilities[i].Use(this.gameObject);
            //     }
            // }
        }

        private void OnDestroy()
        {
            foreach (var handle in _handles)
            {
                AddressableLoader.Release(handle);
            }
        }

        public void AddAbility(string path)
        {
            StartCoroutine(CoAddAbility(path));
        }

        private IEnumerator CoAddAbility(string path)
        {
            var r = AddressableLoader.LoadAsset<Ability>(path);
            yield return r.Wait();
            Ability ability = r.Result;
            if (!string.IsNullOrEmpty(ability.tag))
            {
                _abilityCache[ability.tag] = ability;
            }
        }

        public void ExecuteAbilityByTag(string tag)
        {
            if (_abilityCache.ContainsKey(tag) && _isLoaded[tag])
                _abilityCache[tag].Use(this.gameObject);
        }

        IEnumerator CoLoad()
        {
            // _loadedAbilities = new Ability[abilities.Length];
            // _isLoaded = new bool[abilities.Length];
            _handles = new AsyncOperationHandle<Ability>[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                _handles[i] = Addressables.LoadAssetAsync<Ability>(abilities[i]); 
                yield return _handles[i];
                Ability ability = _handles[i].Result;

                // _isLoaded[i] = true;
                // _loadedAbilities[i] = ability;
                if (!string.IsNullOrEmpty(ability.tag))
                {
                    _abilityCache[ability.tag] = ability;
                    _isLoaded[ability.tag] = true;
                }
                ability.Init(gameObject);

                // _loadedAbilities[i].Init(gameObject);
            }
            yield return null;
        }

    }

}