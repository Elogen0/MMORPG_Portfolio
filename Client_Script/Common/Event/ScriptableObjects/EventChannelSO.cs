using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class EventChannelSO : DescriptionBaseSO
{
    public static T Get<T>(string path, bool resources = true) where T : EventChannelSO
    {
        if (resources)
            return Resources.Load<T>(path);
        else
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
    }
}
