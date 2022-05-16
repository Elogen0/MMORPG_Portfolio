using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestAddressables : MonoBehaviour
{
    [SerializeField] private AssetReference slotPrefab = null;
    private void Awake()
    {
        var go = Addressables.InstantiateAsync(slotPrefab, transform).WaitForCompletion();
    }
}
