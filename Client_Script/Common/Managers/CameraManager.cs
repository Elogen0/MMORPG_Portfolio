using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Kame.Define;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera uiCamera;
    public CinemachineFreeLook freeLookVCam;
    public CinemachineImpulseSource impulseSource;
    
    [SerializeField][Range(.5f, 3f)] private float _speedMultiplier = 1f; //TODO: make this modifiable in the game settings
    
    private TransformAnchor _cameraTransformAnchor = default;
    private TransformAnchor _protagonistTransformAnchor= default;
    private VoidEventChannelSO _camShakeEvent = default;
    private BoolEventChannelSO _controlLockEvent = default;
    
    private bool _isRMBPressed;
    private bool _cameraMovementLock = false;
    private void Awake()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
        _cameraTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
        _protagonistTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor);
        
        _controlLockEvent = EventChannelSO.Get<BoolEventChannelSO>(ResourcePath.ControlLock);
        _camShakeEvent = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.CameraShake);
    }

    private void OnEnable()
    {
        _protagonistTransformAnchor.OnAnchorProvided += SetupProtagonistVirtualCamera;
        _cameraTransformAnchor.Provide(mainCamera.transform);
        
        _camShakeEvent.OnEventRaised += impulseSource.GenerateImpulse;
        _controlLockEvent.OnEventRaised += OnDialogueEvent;
    }

    private void OnDisable()
    {
        _protagonistTransformAnchor.OnAnchorProvided -= SetupProtagonistVirtualCamera;
        _cameraTransformAnchor.Unset();
        
        _camShakeEvent.OnEventRaised -= impulseSource.GenerateImpulse;
        _controlLockEvent.OnEventRaised -= OnDialogueEvent;
    }

    private void Start()
    {
        //Setup the camera target if the protagonist is already available
        if(_protagonistTransformAnchor.isSet)
            SetupProtagonistVirtualCamera();
    }

    private void FixedUpdate()
    {
        Vector2 mouseMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        OnCameraMove(mouseMove);
    }

    private void OnDialogueEvent(bool activated)
    {
        _cameraMovementLock = activated;

        // Cursor.visible = activated;
        // Cursor.lockState = activated ? CursorLockMode.None : CursorLockMode.Locked;

        if (!activated)
        {
            freeLookVCam.m_XAxis.m_InputAxisValue = 0;
            freeLookVCam.m_YAxis.m_InputAxisValue = 0;    
        }
    }
    
    IEnumerator DisableMouseControlForFrame()
    {
        _cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        _cameraMovementLock = false;
    }

    
    private void OnDisableMouseControlCamera()
    {
        _isRMBPressed = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // when mouse control is disabled, the input needs to be cleared
        // or the last frame's input will 'stick' until the action is invoked again
        freeLookVCam.m_XAxis.m_InputAxisValue = 0;
        freeLookVCam.m_YAxis.m_InputAxisValue = 0;
    }
    
    private void OnCameraMove(Vector2 cameraMovement)
    {
        if (_cameraMovementLock)
            return;

        // if (isDeviceMouse && !_isRMBPressed)
        //     return;

        //Using a "fixed delta time" if the device is mouse,
        //since for the mouse we don't have to account for frame duration
        //float deviceMultiplier = isDeviceMouse ? 0.02f : Time.deltaTime;
        float deviceMultiplier = 1f;
        freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * deviceMultiplier * _speedMultiplier;
        freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * deviceMultiplier * _speedMultiplier;
    }
    
    public void SetupProtagonistVirtualCamera()
    {
        Transform target = _protagonistTransformAnchor.Value;

        freeLookVCam.Follow = target;
        freeLookVCam.LookAt = target;
        freeLookVCam.OnTargetObjectWarped(target, target.position - freeLookVCam.transform.position - Vector3.forward);
    }
}
