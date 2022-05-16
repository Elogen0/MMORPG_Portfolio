using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Base class for ScriptableObjects that need a public description field.
/// </summary>
public abstract class DescriptionBaseSO : SerializableScriptableObject
{
    [TextArea] public string description;
    
    
}