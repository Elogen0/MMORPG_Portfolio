using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Game.Data;
using JetBrains.Annotations;
using Kame;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : CreatureController
{
    protected override void MoveToTarget()
    {
        transform.position = targetPosition;
    }

    protected override void UpdateIdleState()
    {
        base.UpdateIdleState();
        _anim.SetFloat(AnimatorHash.Move, 0);
    }


    protected override void UpdateMoveState()
    {
        base.UpdateMoveState();
        _anim.SetFloat(AnimatorHash.Move, 1f);
        
    }

    protected override void UpdateSprintState()
    {
        base.UpdateSprintState();
        _anim.SetFloat(AnimatorHash.Move, 1.5f);
    }

    protected override void UpdateSkillState()
    {
        base.UpdateSkillState();
        _anim.SetFloat(AnimatorHash.Move, 0);
    }
}
