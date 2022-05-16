using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CanvasPixelToUIKitSize : MonoBehaviour
{
    private CanvasScaler _canvasScaler;

    private void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        Resize();
    }

    private void Resize()
    {
        if (Application.isEditor)
        {
            return;
        }

        _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
#if UINITY_ANDROID
        _canvasScaler.scaleFactor = Screen.dpi / 160;
#elif UINITY_IOS
        _canvasScaler.scaleFactor = ApplePlugin.GetNativeScaleFactor();
#endif
    }
}