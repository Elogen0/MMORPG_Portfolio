using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

public class UI_ControlLock : MonoBehaviour
{
    private BoolEventChannelSO _controlLockEvent = default;

    private void Awake()
    {
        _controlLockEvent = EventChannelSO.Get<BoolEventChannelSO>(ResourcePath.ControlLock);
    }

    private void OnEnable()
    {
        _controlLockEvent.RaiseEvent(true);
    }

    private void OnDisable()
    {
        _controlLockEvent.RaiseEvent(false);
    }
}
