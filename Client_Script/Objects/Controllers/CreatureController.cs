using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Abilities;
using Kame.Game.Data;
using UnityEngine;

public class CreatureController : BaseController
{
    private AbilityController _abilityController;
    public void OnDead(Health health)
    {
        ChangeState(CreatureState.Dead);
    }

    public virtual void UseSkill(SkillInfo skillInfo)
    {
        if (DataManager.SkillDict.TryGetValue(skillInfo.SkillId, out SkillData skillData))
        {
            _abilityController.ExecuteAbilityByTag(skillData.tag);
            ChangeState(CreatureState.Skill);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _health.showHealthBar = true;
        _abilityController = GetComponent<AbilityController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isDead = false;
        PosInfo.State = CreatureState.Idle;
        _health.OnDeadEvent += OnDead;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PosInfo.State = CreatureState.Idle;
        _health.OnDeadEvent -= OnDead;
    }
    
    public void SendAnimParameter(AnimatorParameterType parameterType, int hashKey, float value)
    {
        C_AnimParameter parameter = new C_AnimParameter
        {
            Type = parameterType,
            HashKey = hashKey,
            Value = parameterType == AnimatorParameterType.AnimFloat ? (int)(value * 1000) : (int)value
        };

        NetworkManager.Instance.Send(parameter);
    }

    public void ReceiveAnimParameter(S_AnimParameter parameter)
    {
        switch (parameter.Type)
        {
            case AnimatorParameterType.AnimFloat:
                _anim.SetFloat(parameter.HashKey, (float)parameter.Value / 1000);
                break;
            case AnimatorParameterType.AnimInt:
                _anim.SetInteger(parameter.HashKey, parameter.Value);
                break;
            case AnimatorParameterType.AnimBool:
                _anim.SetBool(parameter.HashKey, Convert.ToBoolean(parameter.Value));
                break;
            case AnimatorParameterType.AnimTrigger:
                _anim.SetTrigger(parameter.HashKey);
                break;
        }
    }
}
