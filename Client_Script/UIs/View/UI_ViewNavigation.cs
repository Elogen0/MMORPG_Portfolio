using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Define;
using UnityEngine;

public class UI_ViewNavigation : SceneSingletonMono<UI_ViewNavigation>
{
    [SerializeField] private string entryTag;
    private Stack<string> viewStack = new Stack<string>();

    private UIViewEventChannelSO _uiViewEvent;
    public string CurrentViewName
    {
        get
        {
            if (viewStack.Count > 0)
                return viewStack.Peek();
            return string.Empty;    
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _uiViewEvent = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
    }

    private void Start()
    {
        if (viewStack.Count > 0)
        {
            string viewName = viewStack.Pop();
            Show(viewName);
        }
        else
        {
            Show(entryTag);
        }

        Dictionary<string, UI_View> dic = _uiViewEvent.ViewDic;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var uiView in _uiViewEvent.ViewDic.Values)
            {
                if (Input.GetKeyDown(KeyCode.None))
                    return;
                if (Input.GetKeyDown(uiView.bindingKey))
                {
                    if (uiView.gameObject.activeSelf == false)
                    {
                        Show(uiView.BindingTag);
                        return;
                    }
                    else
                    {
                        Show(entryTag);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("UIVeiw Home");
                    Back();
                }
            }
        }
    }

    public UI_View Show(string viewName)
    {
        if (!_uiViewEvent.ViewDic.ContainsKey(viewName))
        {
            return null;
        }
        
        viewStack.Push(viewName);
        UI_View currentView = GetView(CurrentViewName);
        if (currentView == null)
            return null;

        if (currentView)
        {
            if (currentView.State == UIViewState.Appearing || currentView.State == UIViewState.Disappearing)
                return null;
        }    

        if (viewName == entryTag)
        {
            viewStack.Clear();
            viewStack.Push(entryTag);
        }
        
        _uiViewEvent.RaiseEvent(viewName);
        foreach (var view in _uiViewEvent.ViewDic.Values)
        {
            StartCoroutine(view.ChangeView(viewName));
        }
        return GetView(viewName);
    }

    public UI_View Back()
    {
        if (string.IsNullOrEmpty(CurrentViewName))
            return null;
        
        if (viewStack.Count == 0)
            return Show(entryTag);

        if (GetView(CurrentViewName).isContentRoot)
        {
            return Show(entryTag);
        }
        string currView = viewStack.Pop();
        string prevView = viewStack.Pop();
        return Show(prevView);
    }

    public UI_View Home()
    {
        return Show(entryTag);
    }

    public UI_View GetView(string viewName)
    {
        return _uiViewEvent.GetView(viewName);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void DisplayViewStack()
    {
#if UNITY_EDITOR
        Debug.Log("ViewStack");
        foreach (var viewName in viewStack)
        {
            Debug.Log(viewName);
        }
        Debug.Log("========");
#endif
    }
}
