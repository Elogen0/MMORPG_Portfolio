using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Google.Protobuf.Protocol;
using Kame;
using Kame.Define;
using Kame.Sounds;
using UnityEngine;

public class InputTest : PlayerController
{
    private TransformAnchor _cameraTransform;
    private Rigidbody _rigid;
    private float horizontal;
    private float vertical;

    private Vector3 _moveDirection;

    private bool canDash = true;
    private bool dashing = false;
    private WaitForSeconds dashTime = new WaitForSeconds(.2f);
    private WaitForSeconds dashcoolDown = new WaitForSeconds(1.3f);
    private Vector3 DashDirection;

    #region MonoBehaviours
    protected override void Awake()
    {
        base.Awake();
        _rigid = GetComponent<Rigidbody>();
        
        _rigid = GetComponent<Rigidbody>();
        _rigid.useGravity = true;
        _rigid.isKinematic = false;
        
        gameObject.GetOrAddComponent<ComboController>();
        gameObject.GetOrAddComponent<Shopper>();
        
        GetComponent<CapsuleCollider>().isTrigger = false;
    }

    protected void Start()
    {
        GetComponent<Health>().OnDeadEvent += OnDead;
        GetComponentInChildren<InteractionTrigger>().enabled = true;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<Health>().InitStat(new StatInfo(){CharacterId = 1, Exp = "10", Hp = 200, Level = 1});
        TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor).Provide(transform);
        _cameraTransform = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor).Unset();
    }

    protected void FixedUpdate()
    {
        if (PosInfo.State != CreatureState.Move && PosInfo.State != CreatureState.Sprint)
            return;
        
        DoRotate(_moveDirection);
        if (dashing)
        {
            _rigid.MovePosition(_rigid.position + DashDirection * (_health.Speed * 2f * Time.deltaTime));
            return;
        }
        
        if (PosInfo.State == CreatureState.Sprint)
        {
            _moveDirection *= 1.3f;
        }
        
        _rigid.MovePosition(_rigid.position + _moveDirection * (_health.Speed * Time.deltaTime));
    }
    
    protected override void Update()
    {
        HandleInput();
        _moveDirection = GetDirectionOfCameraView(vertical, horizontal);
        base.Update();
    }
    #endregion
    
    #region StateMachine
    protected override void UpdateIdleState()
    {
        base.UpdateIdleState();
        
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            return;
        }
        if (!Mathf.Approximately(horizontal + vertical, float.Epsilon))
        {
            ChangeState(CreatureState.Move);
        }
    }
    
    protected override void UpdateMoveState()
    {
        base.UpdateMoveState();
        
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            return;
        }
        if (Mathf.Approximately(Mathf.Abs(horizontal) + Mathf.Abs(vertical), float.Epsilon))
        {
            ChangeState(CreatureState.Idle);
            return;
        }
        if (canDash && Input.GetKey(KeyCode.LeftShift))
        {
            ChangeState(CreatureState.Sprint);
        }
    }
    
    

    
    protected override void UpdateSprintState()
    {
        base.UpdateSprintState();
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            return;
        }
        if (Mathf.Approximately(Mathf.Abs(horizontal) + Mathf.Abs(vertical), float.Epsilon))
        {
            ChangeState(CreatureState.Idle);
            return;
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ChangeState(CreatureState.Move);
        }
    }
    
    protected override void OnEnterState()
    {
        switch (PosInfo.State)
        {
            case CreatureState.Idle:
                break;
            case CreatureState.Move:
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                isDead = true;
                break;
            case CreatureState.Jump :
                break;
            case CreatureState.Sprint:
                DashDirection = _moveDirection;
                StartCoroutine(CoDash());
                break;
        }
    }

    #endregion


    #region Move

    void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }
    protected override void MoveToTarget()
    {
        
    }
    
    IEnumerator CoDash()
    {
        canDash = false;
        dashing = true;
        yield return dashTime;
        dashing = false;
        yield return dashcoolDown;
        canDash = true;
    }
    
    public Vector3 GetDirectionOfCameraView(float vertical, float horizontal)
    {
        Vector3 forward = _cameraTransform.Value.TransformDirection(Vector3.forward);
        
        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        Vector3 targetDirection = Vector3.zero;
        targetDirection = forward * vertical + right * horizontal;
        return targetDirection;
    }

    public void DoRotate(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion newRotation = Quaternion.Slerp(_rigid.rotation,
            targetRotation, 0.3f);
        _rigid.MoveRotation(newRotation);
        _rigid.angularVelocity = Vector3.zero;
    }
    
    
    #endregion
    
    
}
