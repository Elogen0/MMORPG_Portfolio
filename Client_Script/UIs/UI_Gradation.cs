using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Gradation : MonoBehaviour
{
    private WaitForSeconds interval = new WaitForSeconds(.1f);
    float duration = .5f;

    private void OnEnable()
    {
        StartCoroutine(CoShowGradation());
    }

    IEnumerator CoShowGradation()
    {
        List<Color> savedAlpha = new List<Color>();
        foreach (RectTransform child in transform)
        {
            if (child.TryGetComponent(out TextMeshProUGUI tmp))
            {
                savedAlpha.Add(tmp.color);
                tmp.alpha = 0;
            }
            else
            {
                var canvasGroup = child.gameObject.GetOrAddComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
            }
        }

        int i = 0;
        foreach (RectTransform child in transform)
        {
            if (child.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.DOFade(savedAlpha[i].a, duration) ;
                ++i;
            }
            else
            {
                var canvasGroup = child.gameObject.GetOrAddComponent<CanvasGroup>();
                var s = DOTween.Sequence();
                s.Append(canvasGroup.DOFade(1, duration));
            }
            yield return interval;
        }
        yield return null;
    }
}
