using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraFixedAspectRatio : MonoBehaviour
{
    [Header("Aspect Ratio")]
    public int width = 16;
    public int height = 9;

    private int currentWidth;
    private int currentHeight;
    private Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustScreen();
    }

    private void Update()
    {
        if (currentWidth != Screen.width || currentHeight != Screen.height)
        {
            AdjustScreen();
        }
    }

    void AdjustScreen()
    {
        currentHeight = Screen.height;
        currentWidth = Screen.width;
        Rect rect = cam.rect;
        float scaleheight = ((float) currentWidth / currentHeight) / ((float) width / height); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }

        cam.rect = rect;
    }

    void OnPreCull() => GL.Clear(true, true, Color.black);
}
