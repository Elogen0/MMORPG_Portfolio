using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.AI
{
    [CreateAssetMenu(menuName = "PluggableAI/State")]
    public class StateSO : ScriptableObject
    {
        public ActionStrategy[] actions;
        public Transition[] transitions;
        public Color sceneGizmoColor = Color.gray;

        public void UpdateState(StateController controller)
        {
            DoActions(controller);
            CheckTransitions(controller);
        }

        private void DoActions(StateController controller)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Act(controller);
            }
        }

        private void CheckTransitions(StateController controller)
        {
            if (controller.anyState)
            {
                for (int i = 0; i < controller.anyState.transitions.Length; i++)
                {
                    if (controller.anyState.transitions[i].decision.Decide(controller))
                    {
                        controller.ChangeState(controller.anyState.transitions[i].trueState);
                    }
                    else
                    {
                        controller.ChangeState(controller.anyState.transitions[i].falseState);
                    }
                }
            }

            for (int i = 0; i < transitions.Length; i++)
            {
                bool decisionSuceeded = transitions[i].decision.Decide(controller);

                if (decisionSuceeded)
                {
                    controller.ChangeState(transitions[i].trueState);
                }
                else
                {
                    controller.ChangeState(transitions[i].falseState);
                }
            }
        }

        public void OnEnterState(StateController controller)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].OnEnterAction(controller);
            }
            for(int i = transitions.Length -1; i >= 0; i--)
            {
                transitions[i].decision.OnEnterDecision(controller);
            }
        }

        public void OnExitState(StateController controller)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].OnExitAction(controller);
            }
            for (int i = transitions.Length - 1; i >= 0; i--)
            {
                transitions[i].decision.OnExitDecision(controller);
            }
        }
    }

}