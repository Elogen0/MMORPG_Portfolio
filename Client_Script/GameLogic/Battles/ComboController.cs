using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Abilities;
using Kame.Define;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    public bool activate = false;
    public bool SendNetwork = false;
    private TransformAnchor _cameraTransform;
    private bool comboInputEnabled = false;
    private Animator _anim;
    private Rigidbody _rigid;
    private AbilityController _ability;
    private bool _dashing;
    private Vector3 DashigDirection;
    private VoidEventChannelSO _cameraShakeEvent;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _ability = GetComponent<AbilityController>();
    }

    private void Start()
    {
        _cameraTransform = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
        _cameraShakeEvent = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.CameraShake);
    }

    public void EnableAttackInput()
    {
        comboInputEnabled = true;
    }
    
    public void SetNextAttack(string name)
    {
        comboInputEnabled = false;
 
        if (_anim.GetBool(AnimatorHash.IsCombo))
        {
            _anim.SetBool(AnimatorHash.IsCombo, false);
            _anim.SetBool(AnimatorHash.IsAttacking, true);
            
            _ability.ExecuteAbilityByTag(name);
            //_anim.CrossFade(name, .2f);
        }
    }

    private void Update()
    {
        if (!activate)
            return;
        if (comboInputEnabled && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SetCombo");
            _anim.SetBool(AnimatorHash.IsCombo, true);
        }
        else if (!_anim.GetBool(AnimatorHash.IsAttacking) && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SetAttack");
            //_anim.CrossFade("attack01", .2f);
            GetComponent<AbilityController>().ExecuteAbilityByTag("attack01");
        }
        
        if (_dashing)
        {
            _rigid.MovePosition(_rigid.position + DashigDirection * Time.deltaTime);
            DoRotate(DashigDirection);
            return;
        }
    }
    
    public void DashX(float speed)
    {
        _dashing = true;
        DashigDirection = GetDirectionOfCameraView(1, 0);
        DashigDirection *= speed;
    }

    public void EndDash()
    {
        _dashing = false;
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
    }

    public void GenerateImpulse()
    {
        _cameraShakeEvent.RaiseEvent();
    }

    public void OnAttackStateUpdate()
    {
        _anim.SetBool(AnimatorHash.IsAttacking, true);
        if (SendNetwork)
        {
            GetComponent<CreatureController>().SendAnimParameter(AnimatorParameterType.AnimBool, AnimatorHash.IsAttacking, 1);
        }
    }

    public void OnAttackStateEnter()
    {
        _anim.SetBool(AnimatorHash.IsAttacking, true);
        if (SendNetwork)
        {
            GetComponent<CreatureController>().SendAnimParameter(AnimatorParameterType.AnimBool, AnimatorHash.IsAttacking, 1);
        }
    }

    public void OnAttackStateExit()
    {
        _anim.SetBool(AnimatorHash.IsCombo, false);
        _anim.SetBool(AnimatorHash.IsAttacking, false);
        GetComponent<BaseController>().ChangeState(CreatureState.Idle);
        if (SendNetwork)
        {
            GetComponent<CreatureController>().SendAnimParameter(AnimatorParameterType.AnimBool, AnimatorHash.IsAttacking, 0);
            GetComponent<CreatureController>().SendAnimParameter(AnimatorParameterType.AnimBool, AnimatorHash.IsCombo, 0);
        }
    }

   
}
