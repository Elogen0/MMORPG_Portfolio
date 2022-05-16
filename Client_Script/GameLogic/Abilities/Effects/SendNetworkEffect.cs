using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Abilities;
using Kame.Define;
using Kame.Game.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "New Send Network Effect", menuName = "Abilities/Effects/SendNetwork", order = 0)]
public class SendNetworkEffect : EffectStrategy
{
    [SerializeField] private int skillId;
    
    private TransformAnchor _transformAnchor;

    public override void Init()
    {
        _transformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
    }

    public override void StartEffect(AbilityData data, Action finished)
    {
        if (data.User.TryGetComponent(out AbilityController controller))
        {
            if (!controller.SendNetwork)
                return;
        }
        else
        {
            return;
        }
        
        
        Debug.Log("SendNetwork Effect");
        if (DataManager.SkillDict.TryGetValue(skillId, out SkillData skillData))
        {
            Vector3 dirVec = _transformAnchor.Value.TransformDirection(Vector3.forward);
            dirVec = Vector3.ProjectOnPlane(dirVec, Vector3.up).normalized;
            // if (transformAnchor && transformAnchor.isSet)
            // {
            //     dirVec = 
            // }
            // else
            // {
            //     dirVec = data.User.transform.TransformDirection(Vector3.forward);
            // }
            Debug.Log($"SendSkill id = {skillId}");
            C_Skill skill = new C_Skill
            {
                Info = new SkillInfo 
                {
                    SkillId = this.skillId,
                    PosInfo = new PositionInfo()
                    {
                        DirX = dirVec.x,
                        DirY = dirVec.z,
                        PosX = data.TargetedPoint.x,
                        PosY = data.TargetedPoint.z,
                        PosH = data.TargetedPoint.y,
                        State = CreatureState.Skill,
                    }    
                }
            };
            NetworkManager.Instance.Send(skill);
        }
    }
}
