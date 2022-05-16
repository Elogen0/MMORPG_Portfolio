using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackgroundChanger : MonoBehaviour
{
    [Serializable]
    public class BackGroundBinder
    { 
        public Sprite background;
        public string[] tag;
    }

    [SerializeField] private BackGroundBinder[] binders;

    private Image backgroundImage;
    private UIViewEventChannelSO _uiViewEvent;
    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        _uiViewEvent = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
    }

    private void Start()
    {
        _uiViewEvent.OnEventRaised += On;
    }

    private void OnDestroy()
    {
        _uiViewEvent.OnEventRaised -= On;
    }

    public void On(string viewName)
    {
        foreach (var backGroundBinder in binders)
        {
            foreach (var tag in backGroundBinder.tag)
            {
                if (tag == viewName)
                {
                    if (backgroundImage.sprite == backGroundBinder.background)
                    {
                        return;
                    }
                    backgroundImage.sprite = backGroundBinder.background;
                    return;
                }
            }
        }
    }
}
