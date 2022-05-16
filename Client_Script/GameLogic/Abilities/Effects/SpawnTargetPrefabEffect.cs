using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Target Prefab Effect", menuName = "Abilities/Effects/SpawnPrefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] AssetReference prefabToSpawn;
        [SerializeField] float destroyDelay = -1;
        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Effect(data, finished));
        }

        private IEnumerator Effect(AbilityData data, Action finished)
        {
            var request = AddressableLoader.Instantiate(prefabToSpawn);
            yield return request.Wait();
            GameObject go = request.Result;
            go.transform.position = data.TargetedPoint;
            if (destroyDelay > 0)
            {
                AddressableLoader.ReleaseInstance(go);
            }
            finished();
            yield break;
        }
    }

}