using System;
using System.Collections;
using Cinemachine.Utility;
using Google.Protobuf.Protocol;
using Kame.Game;
using Kame;
using Kame.Abilities;
using Kame.Define;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    public PlayerInventory Inven { get; protected set; }
    public PlayerEquipment Equip { get; protected set; }
    
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

    private Vector3 prevPosition = Vector3.zero;
    #region MonoBehaviours
    protected override void Awake()
    {
        base.Awake();
        //
        _rigid = GetComponent<Rigidbody>();
        _rigid.useGravity = true;
        _rigid.isKinematic = false;
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetComponent<AbilityController>().SendNetwork = true;
        gameObject.GetOrAddComponent<ComboController>().activate = true;
        gameObject.GetComponent<ComboController>().SendNetwork = true;
        
        Inven = gameObject.GetOrAddComponent<PlayerInventory>();
        Equip = gameObject.GetOrAddComponent<PlayerEquipment>();
        gameObject.GetOrAddComponent<Shopper>();
    }

    private void Start()
    {
        GetComponentInChildren<InteractionTrigger>().enabled = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
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
    
    private void LateUpdate()
    {
        float value = (prevPosition - transform.position).magnitude;

        if (value > 0.1f)
        {
            SendPosInfo();
            prevPosition = transform.position;
        }
    }
    #endregion
    
    #region StateMachine
    protected override void UpdateIdleState()
    {
        base.UpdateIdleState();
        
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            SendPosInfo();
            return;
        }

        if (!Mathf.Approximately(horizontal + vertical, float.Epsilon))
        {
            ChangeState(CreatureState.Move);
            SendPosInfo();
        }
    }

    protected override void UpdateMoveState()
    {
        base.UpdateMoveState();
        
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            SendPosInfo();
            return;
        }
        if (Mathf.Approximately(Mathf.Abs(horizontal) + Mathf.Abs(vertical), float.Epsilon))
        {
            ChangeState(CreatureState.Idle);
            SendPosInfo();
            return;
        }
        if (canDash && Input.GetKey(KeyCode.LeftShift))
        {
            ChangeState(CreatureState.Sprint);
            SendPosInfo();
        }
    }
    
    protected override void UpdateSprintState()
    {
        base.UpdateSprintState();
        
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            ChangeState(CreatureState.Skill);
            SendPosInfo();
            return;
        }
        if (Mathf.Approximately(Mathf.Abs(horizontal) + Mathf.Abs(vertical), float.Epsilon))
        {
            ChangeState(CreatureState.Idle);
            SendPosInfo();
            return;
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ChangeState(CreatureState.Move);
            SendPosInfo();
            return;
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
    
    public Inventory GetInventoryOfType(InventoryType type)
    {
        if (type == InventoryType.Equipment)
            return Equip.Container;
        if (type == InventoryType.Inventory)
            return Inven.Container;
        return null;
    }
    public override void UseSkill(SkillInfo skillInfo)
    {
        
    }
    
    public override void InitStat(StatInfo value)
    {
        base.InitStat(value);
        Equip.ReCalculateItemStat();
    }

    #region Move
    protected override void MoveToTarget()
    {
        
    }
     
    void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
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
    
    protected void SendPosInfo()
    {
        Vector3 curPos = transform.position;
        Vector3 dirVec = transform.TransformDirection(Vector3.forward);
        C_Move movePacket = new C_Move {PosInfo = new PositionInfo
        {
            PosX = curPos.x,
            PosY = curPos.z,
            PosH = curPos.y,
            DirX = dirVec.x,
            DirY = dirVec.z,
            State = nextState
        }};
        NetworkManager.Instance.Send(movePacket);
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 100, 100), $" currentState : {PosInfo.State.ToString()}, nextState : {nextState}");
    }
#endif
}
