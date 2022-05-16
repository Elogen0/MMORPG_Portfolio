using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Runtime Anchors/Transform")]
public class TransformAnchor : RuntimeAnchorBase<Transform>
{
    public static T Get<T>(string path, bool resources = true) where T : TransformAnchor
    {
        if (resources)
            return Resources.Load<T>(path);
        else
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
    }
}
