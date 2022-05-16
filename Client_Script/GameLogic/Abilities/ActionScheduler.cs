using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Core
{

    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        public void StartAction(IAction action)
        {
            if (currentAction == action) return;
            currentAction?.Cancel();
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }

}