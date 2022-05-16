using Kame.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Follow Prefab Effect", menuName = "Abilities/Effects/FollowPrefab", order = 0)]
public class SpawnFollowPrefabEffect : EffectStrategy
{
    [SerializeField] Transform prefabToSpawn;
    [SerializeField] float destroyDelay = -1;
    public override void Init()
    {
    }

    public override void StartEffect(AbilityData data, Action finished)
    {
    }

    private IEnumerator Effect(AbilityData data, Action finished)
    {
        List<Transform> instances = new List<Transform>();
        foreach (var target in data.Targets)
        {
            instances.Add(Instantiate(prefabToSpawn));
        }

        float elapseTime = 0f;
        while(elapseTime < destroyDelay)
        {
            int i = 0;
            foreach (var target in data.Targets)
            {
                instances[i].position = target.transform.position;
                ++i;
            }
            
            yield return null;
            elapseTime += Time.deltaTime;
        }
        finished();

        if (destroyDelay > 0)
        {
            foreach (var instance in instances)
            {
                Destroy(instance.gameObject);
            }
        }
    }
}
