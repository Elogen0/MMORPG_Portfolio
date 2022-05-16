using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    public float yOffset = default;
    private TransformAnchor _audioListenerTransformAnchor = default;
    private TransformAnchor _cameraTransformAnchor = default;
    private TransformAnchor _protagonistTransformAnchor= default;
    
    private void Awake()
    {
        _audioListenerTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.AudioListenerAnchor);
        _cameraTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
        _protagonistTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor);
    }

    private void OnEnable()
    {
        _audioListenerTransformAnchor.Provide(transform);
    }

    private void OnDisable()
    {
        _audioListenerTransformAnchor.Unset();
    }

    private void LateUpdate()
    {
        if (_cameraTransformAnchor.isSet && _protagonistTransformAnchor.isSet)
        {
            transform.position = _protagonistTransformAnchor.Value.position + Vector3.up * yOffset;
            transform.forward = _cameraTransformAnchor.Value.forward;
        }
    }
}
