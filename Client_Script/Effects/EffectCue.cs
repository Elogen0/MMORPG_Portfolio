using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EffectCue : MonoBehaviour
{
    [SerializeField] private AssetReference[] effectList;

    public void SpawnEffect(int index)
    {
        if (index >= effectList.Length)
            return;
        StartCoroutine(CoEffect(index));
    }

    IEnumerator CoEffect(int index)
    {
        var r = AddressableLoader.InstantiatePooling(effectList[index], transform.position, transform.rotation);
        yield return r.Wait();
        if (r.Result.TryGetComponent(out ParticleSystem particle))
        {
            if (particle.isPaused)
                particle.Play();
        }
    }
}
