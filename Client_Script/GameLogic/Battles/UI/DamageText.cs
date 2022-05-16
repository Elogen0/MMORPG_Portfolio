using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kame;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
public class DamageText : MonoBehaviour, Spawnee
{
    public const string Path = "Assets/Game/Prefab/UI/UI_DamageText.prefab";
    private TextMeshProUGUI _textMesh;
    
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        
    }

    public IEnumerator AfterSpawnAction(GameObject caller, object data)
    {
        _textMesh.text = (string) data;
        _textMesh.alpha = 0;
        float rand = Random.Range(-0.3f, 0.3f);
        transform.position += new Vector3(rand, rand, rand);
        Sequence s = DOTween.Sequence()
            .Append(transform.DOScale(_textMesh.transform.localScale * 2, .4f).From().SetEase(Ease.OutQuad))
            .Insert(0, _textMesh.DOFade(1, .2f))
            .Insert(0, transform.DOMoveY(transform.position.y + .5f , 1f).SetEase(Ease.OutCirc))
            //.Append(transform.DOMoveY(transform.position.y + .6f, .5f).SetEase(Ease.InOutQuad))
            .Insert(.8f,_textMesh.DOFade(0,.2f).SetEase(Ease.InExpo));
        yield return s.WaitForCompletion();
        _textMesh.text = string.Empty;
        AddressableLoader.ReleaseInstance(gameObject);
        yield return null;
    }
}
