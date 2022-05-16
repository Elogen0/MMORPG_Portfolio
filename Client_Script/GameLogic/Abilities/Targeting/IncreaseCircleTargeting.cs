using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kame;
using Kame.Abilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kame.Abilities
{
    [CreateAssetMenu(fileName = "New IncreaseCircle Targeting", menuName = "Abilities/Targeting/IncreaseCircleTargeting", order = 0)]
    public class IncreaseCircleTargeting : TargetingStrategy
    {
        [SerializeField] private float range;
        [SerializeField] private float duration;
        [SerializeField] private AssetReference castingPrefab;
        public override void Init(GameObject user)
        {
        }

        public override void StartTargeting(AbilityData data, Action finished)
        {
            data.StartCoroutine(IncreaseCircle(data, finished));
        }

        private IEnumerator IncreaseCircle(AbilityData data, Action finished)
        {
            var requestOriginal = AddressableLoader.InstantiatePooling(castingPrefab, data.User.transform.position, data.User.transform.rotation);
            yield return requestOriginal.Wait();
            GameObject goOrigin = requestOriginal.Result;
            goOrigin.transform.localScale = new Vector3( range * 2 , 1, range * 2);
            var request = AddressableLoader.InstantiatePooling(castingPrefab, data.User.transform.position, data.User.transform.rotation);
            yield return request.Wait();
            GameObject go = request.Result;
            float elapsedTime = 0;
            float scale = 0;
            while (!data.IsCancelled())
            {
                if (elapsedTime >= duration)
                {
                    data.Targets = GetGameObjectsInRadius(data.User.transform.position, scale);
                    data.TargetedPoint = data.User.transform.position;
                    break;
                }
                scale = Mathf.Lerp(0.001f, range, elapsedTime / duration);
                go.transform.localScale = new Vector3( scale * 2 , 1, scale * 2);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            AddressableLoader.ReleaseInstance(goOrigin);
            AddressableLoader.ReleaseInstance(go);
            finished();
        }
        
        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point, float range)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, range, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}

