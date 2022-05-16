using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Game.Data;
using Kame.Quests;
using UnityEngine;

public class MonsterController : CreatureController
{
    protected override void OnEnterState()
    {
        switch (PosInfo.State)
        {
            case CreatureState.Idle:
                _anim.CrossFade(AnimatorHash.Idle, .1f);
                break;
            case CreatureState.Move:
                _anim.CrossFade(AnimatorHash.Move, .1f);
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                isDead = true;
                QuestManager.Instance.ProcessQuest(QuestType.DestroyEnemy, Stat.id, 1);
                _anim.CrossFade(AnimatorHash.Death, .1f);
                //GameObject.Destroy(gameObject, 0.5f);
                break;
            case CreatureState.Jump :
                break;
            case CreatureState.Sprint :
                break;
        }
    }

    protected override void MoveToTarget()
    {
        transform.position += (targetPosition - transform.position) * (Time.deltaTime * _health.Speed);
        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _health.Speed);

    }
}
