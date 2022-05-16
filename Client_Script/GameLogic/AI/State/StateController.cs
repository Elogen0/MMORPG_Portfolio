using Kame.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public StateSO initialState;
    public StateSO anyState;
    private StateMachine<StateController> stateMachine;
    
    public float stateElapseTime { get; set; } = 0;
    public Animator Anim { get; set; }
   
    private void Awake()
    {
        stateMachine = new StateMachine<StateController>(this, initialState);
        Anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //if (!aiActive)
        //    return;
        stateMachine.Update(Time.deltaTime);
        stateElapseTime += Time.deltaTime;
    }

    public void ChangeState(StateSO nextState)
    {
        stateMachine.ChangeState(nextState);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (stateMachine?.CurrentState != null )
        {
            Gizmos.color = stateMachine.CurrentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 0.5f);
            Gizmos.color = Color.red;
        }
    }
#endif
}
