using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kame;
using Kame.Define;
using UnityEngine;

public class ObjectOnOffWithTag : MonoBehaviour
{
    [System.Serializable]
    public class ObjectBinder
    {
        public GameObject obj;
        public string[] tags;
    }

    [SerializeField] private ObjectBinder[] binders;
    private UIViewEventChannelSO _uiViewEvent;
    private void Awake()
    {
        foreach (var binder in binders)
        {
            binder.obj.SetActive(false);
        }
        _uiViewEvent = EventChannelSO.Get<UIViewEventChannelSO>(ResourcePath.UIViewEventChannel);
    }
    
    private void OnEnable()
    {
        _uiViewEvent.OnEventRaised += On;
    }

    private void OnDisable()
    {
        _uiViewEvent.OnEventRaised -= On;
    }

    public void On(string viewName)
    {
        foreach (var objectBinder in binders)
        {
            objectBinder.obj.SetActive(false);
            foreach (var tag in objectBinder.tags)
            {
                if (tag == viewName)
                {
                    objectBinder.obj.SetActive(true);
                }
            }
        }
    }
}
