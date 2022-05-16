using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame;
using Kame.Battles;
using Kame.Game.Data;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }
    protected Health _health;
    protected CreatureState nextState = CreatureState.Idle;
    protected bool isDead = false;
    public bool IsDead => isDead;
    public void ChangeState(CreatureState state)
    {
        nextState = state;
    }

    public CharacterStat Stat => _health.Stat;

    public virtual void InitStat(StatInfo value)
    {
        _health.InitStat(value);
    }

    protected PositionInfo _positionInfo = new PositionInfo() {State = CreatureState.Idle};
    protected Vector3 targetPosition;
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;
            _positionInfo.PosX = value.PosX;
            _positionInfo.PosY = value.PosY;
            _positionInfo.PosH = value.PosH;
            //todo : 나중에 State와 Position 변경하는거 따로 뺴놓자...
            nextState = value.State;

            //Change ClientWorld
            targetPosition = new Vector3(value.PosX, value.PosH, value.PosY);
            //transform.position = new Vector3(value.PosX, value.PosH, value.PosY);
            
            transform.LookAt(new Vector3(transform.position.x + value.DirX, transform.position.y,
                transform.position.z + value.DirY));
        }
    }
    
    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosH, PosInfo.PosY);
    }

    protected Animator _anim;
    protected virtual void Awake()
    {
        _health = GetComponent<Health>();
        _anim = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
        //ChangeState(CreatureState.Idle);
    }
    protected virtual void Update()
    {
        if (isDead)
        {
            return;
        }
        MoveToTarget();

        switch (PosInfo.State)
        {
            case CreatureState.Idle :
                UpdateIdleState();
                break;
            case CreatureState.Move :
                UpdateMoveState();
                break;
            case CreatureState.Sprint:
                UpdateSprintState();
                break;
            case CreatureState.Skill:
                UpdateSkillState();
                break;
            case CreatureState.Dead:
                UpdateDeadState();
                break;
            case CreatureState.Jump:
                UpdateJumpState();
                break;
        }
        if (PosInfo.State != nextState)
        {
            OnExitState();
            //Debug.Log($"StateChanged {PosInfo.State} => {nextState}");
            PosInfo.State = nextState;
            OnEnterState();
        }
    }

    protected virtual void MoveToTarget()
    {
        
    }
    
    protected virtual void UpdateIdleState()
    {
        
    }
    
    protected virtual void UpdateMoveState()
    {
        
    }

    protected virtual void UpdateSkillState()
    {
        // if (!_anim.GetBool(AnimatorHash.IsAttacking))
        // {
        //     ChangeState(CreatureState.Idle);
        // }
    }

    protected virtual void UpdateSprintState()
    {
        
    }

    protected virtual void UpdateDeadState()
    {
        
    }

    protected virtual void UpdateJumpState()
    {
        
    }
    protected virtual void OnExitState()
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
                isDead = false;
                break;
            case CreatureState.Jump :
                break;
            case CreatureState.Sprint :
                break;
        }
    }

    protected virtual void OnEnterState()
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
            case CreatureState.Sprint :
                break;
        }
    }
    
    public virtual void TakeDamage(GameObject instigator, float damage)
    {
        _health.TakeDamage(instigator, damage);
        ObjectSpawner.Instance.SpawnAsync(gameObject, DamageText.Path, _health.LookAtPoint, Quaternion.identity, UIManager.Instance.WorldSpaceCanvas.transform, damage.ToString("F0"));
        ObjectSpawner.Instance.SpawnAsync(gameObject, "Assets/Game/Prefab/Effect/HitEffect.prefab", _health.LookAtPoint, Quaternion.identity);
        // StartCoroutine(CoSpawnDamageEffect());
        GetComponent<CreatureSound>().PlayHitVoice();
    }
    
    // private IEnumerator CoSpawnDamageEffect()
    // {
    //     var request = AddressableLoader.InstantiatePooling("Assets/Game/Prefab/Effect/HitEffect.prefab", _health.LookAtPoint, Quaternion.identity);
    //     yield return request.Wait();
    // }
    
    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        switch (nextState)
        {
            case CreatureState.Idle:
                Gizmos.color = Color.white;
                break;
            case CreatureState.Move:
                Gizmos.color = Color.blue;
                break;
            case CreatureState.Skill:
                Gizmos.color = Color.red;
                break;
            case CreatureState.Dead:
                isDead = true;
                Gizmos.color = Color.gray;
                break;
        }
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 3f, .5f);
    }
#endif
}
