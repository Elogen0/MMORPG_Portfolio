using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.AI
{
    public abstract class ActionStrategy : ScriptableObject
    {
        public abstract void Act(StateController controller);

        public virtual void OnEnterAction(StateController controller)
        {

        }

        public virtual void OnExitAction(StateController controller)
        {

        }
    } 
}