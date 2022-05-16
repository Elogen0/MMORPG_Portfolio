using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PointerInfo
{
    /// <summary>
    /// Current pointer position
    /// </summary>
    public Vector2 currentPosition;

    /// <summary>
    /// Previous frame's pointer position
    /// </summary>
    public Vector2 previousPosition;

    /// <summary>
    /// Movement delta for this frame
    /// </summary>
    public Vector2 delta;

    /// <summary>
    /// Tracks if this pointer began over UI
    /// </summary>
    public bool startedOverUI;
}