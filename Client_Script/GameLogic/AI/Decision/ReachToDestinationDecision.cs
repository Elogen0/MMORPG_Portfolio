using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ReachDestination", fileName = "New Reach Destination Decision", order = 0)]
public class ReachToDestinationDecision : DecisionStrategy
{
    public override bool Decide(StateController controller)
    {
        //return controller.naveMeshAgent.remainingDistance <= controller.naveMeshAgent.stoppingDistance;

        //controller.naveMeshAgent.destination == controller.transform.position;
        return false;
    }
}
