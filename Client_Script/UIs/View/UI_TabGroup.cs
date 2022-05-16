using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Kame.Define;
using UnityEngine;

public class UI_TabGroup : MonoBehaviour
{
    [SerializeField] private string entryTag;
    [Header("Button")]
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    
    private Dictionary<string, UI_TabButton> tabButtons = new Dictionary<string, UI_TabButton>();
    private Dictionary<string, UI_TabContents> tabContents = new Dictionary<string, UI_TabContents>();
    private UI_TabButton selectedTab;

    private UIViewEventChannelSO _uiViewEvent;
    private void Awake()
    {
        _uiViewEvent = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
        foreach (var tabButton in transform.GetComponentsInChildren<UI_TabButton>(true))
        {
            tabButtons.Add(tabButton.BindingTag, tabButton);
        }

        foreach (var tabContent in transform.GetComponentsInChildren<UI_TabContents>(true))
        {
            tabContents.Add(tabContent.BindingTag, tabContent);
        }
    }

    private void Start()
    {
        foreach (var button in tabButtons.Values)
        {
            button.background.sprite = tabIdle;
        }
        if (tabButtons.ContainsKey(entryTag))
            OnTabSelected(tabButtons[entryTag]);
    }

    private void OnEnable()
    {
        foreach (var button in tabButtons.Values)
        {
            button.OnTabEnter += OnTabEnter;
            button.OnTabExit += OnTabExit;
            button.OnTabSelected += OnTabSelected;
        }
        _uiViewEvent.OnEventRaised += On;
    }

    private void OnDisable()
    {
        foreach (var button in tabButtons.Values)
        {
            button.OnTabEnter -= OnTabEnter;
            button.OnTabExit -= OnTabExit;
            button.OnTabSelected -= OnTabSelected;
        }
        _uiViewEvent.OnEventRaised -= On;
    }

    public void OnTabEnter(UI_TabButton button)
    {
        ResetTabs();
        if (!selectedTab || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(UI_TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UI_TabButton button)
    {
        if (selectedTab != null)
            selectedTab.Deselect();
        selectedTab = button;
        selectedTab.Select();
        ResetTabs();
        button.background.sprite = tabActive;

        if (tabContents.TryGetValue(selectedTab.BindingTag, out UI_TabContents selectedContent))
        {
            selectedContent.gameObject.SetActive(true);
        }

        foreach (var content in tabContents.Values)
        {
            if (content != selectedContent)
                content.gameObject.SetActive(false);
        }
    }

    public void ResetTabs()
    {
        foreach (var button in tabButtons.Values)
        {
            if (selectedTab && selectedTab == button)
                continue;
            button.background.sprite = tabIdle;
        }
    }

    public void On(string viewName)
    {
        foreach (var button in tabButtons.Values)
        {
            UI_ViewNavigationButton navButton = button as UI_ViewNavigationButton;
            if (!navButton)
                continue;
            if (navButton.BindingTag == viewName)
            {
                OnTabSelected(navButton);
            }
        }
    }
}
