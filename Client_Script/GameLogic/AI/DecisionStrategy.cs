using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionStrategy : ScriptableObject
{
    public abstract bool Decide(StateController controller);

    public virtual void OnEnterDecision(StateController controller)
    {

    }

    public virtual void OnExitDecision(StateController controller)
    {

    }
}
