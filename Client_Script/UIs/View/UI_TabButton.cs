using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [NonSerialized] public Image background;
    [SerializeField] private string bindingTag;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    public Action<UI_TabButton> OnTabEnter;
    public Action<UI_TabButton> OnTabSelected;
    public Action<UI_TabButton> OnTabExit;

    public string BindingTag => bindingTag;
    protected virtual void Awake()
    {
        background = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnTabEnter?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTabSelected?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnTabExit?.Invoke(this);
    }

    public void Select()
    {
        onTabSelected?.Invoke();
    }

    public void Deselect()
    {
        onTabDeselected?.Invoke();
    }
}