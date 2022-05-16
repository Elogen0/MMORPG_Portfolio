using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionText : MonoBehaviour, Spawnee
{
    public const string Path = "Assets/Game/Prefab/InteractText.prefab";
    private TextMesh _textMesh;
    
    private void Awake()
    {
        _textMesh = GetComponent<TextMesh>();
    }

    public IEnumerator AfterSpawnAction(GameObject caller, object data)
    {
        _textMesh.text = (string) data;
        yield return null;
    }
}
