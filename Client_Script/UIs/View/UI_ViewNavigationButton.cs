using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ViewNavigationButton : UI_TabButton
{
    public bool isBackButton;
    
    private Button button;

    public Action<string> OnClickBack;
    protected override void Awake()
    {
        base.Awake();
        if (isBackButton)
        {
            GetComponent<Button>()?.onClick.AddListener(Back);
        }
        else
        {
            GetComponent<Button>()?.onClick.AddListener(ShowView);
        }

    }

    public void ShowView()
    {
        UI_ViewNavigation.Instance.Show(BindingTag);
    }

    public void Back()
    {
        UI_ViewNavigation.Instance.Back();
    }
}
