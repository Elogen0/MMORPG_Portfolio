using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopupBlocker : MonoBehaviour
{
    public void Hide()
    {
        UIManager.Instance.ClosePopup();
    }
}
