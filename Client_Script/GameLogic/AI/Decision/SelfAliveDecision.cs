using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/SelfAlive", fileName = "New SelfAlive Decision", order = 0)]
public class SelfAliveDecision : DecisionStrategy
{
    public override bool Decide(StateController controller)
    {
        return !controller.GetComponent<Health>().IsDead;
    }
}
