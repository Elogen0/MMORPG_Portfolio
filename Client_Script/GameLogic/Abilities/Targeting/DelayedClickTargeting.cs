using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kame.Abilities
{
    [CreateAssetMenu(fileName = "New DelayedTargeting", menuName = "Abilities/Targeting/DelayedClickTargeting", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaAffectRadius;
        [SerializeField] AssetReference targetingPrefab;

        WaitWhile waitClick;

        public override void Init(GameObject user)
        {
            waitClick = new WaitWhile(() => Input.GetMouseButtonDown(0));
        }

        public override void StartTargeting(AbilityData data, Action finished)
        {
            AbilityController controller = data.User.GetComponent<AbilityController>();
            controller.StartCoroutine(Targeting(data, controller, finished));
        }

        private IEnumerator Targeting(AbilityData data, AbilityController controller, Action finished)
        {
            controller.enabled = false;
            var handle =  Addressables.InstantiateAsync(targetingPrefab);
            yield return handle;

            GameObject targetingPrefabInstance = handle.Result;
            while(!data.IsCancelled())
            {
                //serCursor
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit cameraRayHit, 1000, layerMask))
                {
                    targetingPrefabInstance.transform.position = cameraRayHit.point;
                    targetingPrefabInstance.transform.localScale = new Vector3(areaAffectRadius*2, 1, areaAffectRadius * 2);

                    targetingPrefabInstance.gameObject.SetActive(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        //while (Input.GetMouseButton(0))
                        //{
                        //    yield return null;
                        //}

                        // Absorb the whole mouse click
                        yield return waitClick;
                        
                        data.Targets = GetGameObjectsInRadius(cameraRayHit.point);
                        data.TargetedPoint = cameraRayHit.point;
                        break;
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        yield return waitClick;
                        break;
                    }

                }
                yield return null;
            }
            AddressableLoader.ReleaseInstance(targetingPrefabInstance);
            targetingPrefabInstance.gameObject.SetActive(false);
            controller.enabled = true;
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }

}