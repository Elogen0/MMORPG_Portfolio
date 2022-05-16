using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class AssetReferenceTransformAnchor : AssetReferenceT<TransformAnchor>
{
    public AssetReferenceTransformAnchor(string guid) : base(guid)
    {
    }
}
