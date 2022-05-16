using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorHash
{
    //Idle
    public static readonly int IdleIndex = Animator.StringToHash("IdleIndex");
    public static readonly int Idle = Animator.StringToHash("idle");
    
    //Move
    public static readonly int Move = Animator.StringToHash("move");
    public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    public static readonly int IsMove = Animator.StringToHash("IsMove");
    public static readonly int Horizontal = Animator.StringToHash("Horizontal");
    public static readonly int Vertical = Animator.StringToHash("Vertical");
    public static readonly int Footstep = Animator.StringToHash("Footstep");
    public static readonly int Sprint = Animator.StringToHash("sprint");
    public static readonly int Fast = Animator.StringToHash("Fast");

    //Jump
    public static readonly int Falling = Animator.StringToHash("Falling");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    
    //Attack
    public static readonly int InBattle = Animator.StringToHash("InBattle");
    public static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    public static readonly int Attack = Animator.StringToHash("attack");
    public static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    public static readonly int StopAttackTrigger = Animator.StringToHash("StopAttackTrigger");
    public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    public static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
    public static readonly int IsCombo = Animator.StringToHash("isCombo");

    //Damage
    public static readonly int IsAlive = Animator.StringToHash("IsAlive");
    public static readonly int IsDead = Animator.StringToHash("IsDead");
    public static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
    public static readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
    public static readonly int Dead = Animator.StringToHash("Dead");
    public static readonly int Death = Animator.StringToHash("death");
    //Conversation
    public static readonly int Talk = Animator.StringToHash("talk");
    
}
