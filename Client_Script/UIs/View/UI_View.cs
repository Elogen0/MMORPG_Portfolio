using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kame.Define;
using UnityEngine;

public enum UIViewState
{
    Appearing,
    Appeared,
    Disappearing,
    Disappeared
}

[RequireComponent(typeof(CanvasGroup))]
public class UI_View : UI_Base
{
    [SerializeField] string bindingTag;
    [SerializeField] private bool contentRoot;
    public KeyCode bindingKey;
    public bool isContentRoot => contentRoot;
    public UIViewState State { get; set; } = UIViewState.Disappeared;
    
    public Action OnViewDisabled;
    public Action OnViewEndabled;

    private CanvasGroup canvasGroup;
    public string BindingTag => bindingTag;
    private UIViewEventChannelSO _uiViewEvent;
    protected override void Awake()
    {
        if (string.IsNullOrEmpty(bindingTag))
        {
            Debug.LogWarning($"{gameObject.name}(view) is not allocated view name");
        }
        canvasGroup = GetComponent<CanvasGroup>();
        _uiViewEvent = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
        if (!String.IsNullOrEmpty(bindingTag))
        {
            _uiViewEvent.RegisterView(BindingTag, this);
        }
    }

    private void OnDestroy()
    {
        _uiViewEvent.DeRegisterView(BindingTag, this);
    }

    public IEnumerator ChangeView(string viewName)
    {
        if (viewName == BindingTag)
        {
            if (!gameObject.activeSelf)
            {
                OnViewEndabled?.Invoke();
                yield return Show();
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                OnViewDisabled?.Invoke();
                yield return Hide();
            }
        }
    }

    public IEnumerator Show()
    {
        gameObject.SetActive(true);
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        State = UIViewState.Appearing;
        yield return canvasGroup.DOFade(1, .1f).WaitForCompletion();
        State = UIViewState.Appeared;
    }

    public IEnumerator Hide()
    {
        State = UIViewState.Disappearing;
        yield return canvasGroup.DOFade(0, .1f).WaitForCompletion();
        State = UIViewState.Disappeared;
        gameObject.SetActive(false);
    }


    public override void Init()
    {
        
    }
}