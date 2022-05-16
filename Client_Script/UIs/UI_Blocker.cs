using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kame;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI_Blocker : MonoBehaviour
{
    [SerializeField] protected GameObject[] hideObjects;

    protected CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvasGroup.DOFade(1, .2f);
    }

    public void Hide()
    {
        foreach (var obj in hideObjects)
        {
            obj.transform.DOScale(new Vector3(0, 0), .2f).SetEase(Ease.Linear).OnComplete(()=>
            {
                ResourceLoader.Destroy(obj);
            });
        }

        canvasGroup.DOFade(0, .2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            ResourceLoader.Destroy(gameObject);
        });
    }

}
