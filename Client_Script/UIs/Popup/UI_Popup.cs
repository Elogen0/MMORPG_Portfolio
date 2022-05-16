using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Kame.Define;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
public enum UIButtonType
{
    Normal,
    Confirm,
    Cancel,
}

public struct ButtonData
{
    public string text;
    public UIButtonType buttonType;
    public UnityAction action;
}

[RequireComponent(typeof(CanvasGroup))]
public class UI_Popup : UI_Base
{
    enum GameObjects
    {
        tmp_message,
        group_button_container
    }
    
    public void SetMessage(string message)
    {
        GetObject((int) GameObjects.tmp_message).GetComponent<TextMeshProUGUI>().text = message;
    }
    
    public void SetButton(int index, string text, UnityAction action = null, bool hidePopup = true)
    {
        GameObject buttonGroup = GetObject((int) GameObjects.group_button_container);

        Transform children = transform.GetChild(index);
        if (children == null)
        {
            Debug.LogWarning($"{gameObject.name} has not children");
        }
        Button btn = children.GetComponent<Button>();
        if (!btn)
            return;
        btn.gameObject.SetActive(true);
        if (action != null)
        {
            btn.onClick.AddListener(action);
        }
        if (hidePopup)
            btn.onClick.AddListener(UIManager.Instance.ClosePopup);
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
    }
}
