using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using UnityEngine;

[CreateAssetMenu(fileName = "New Self Targeting", menuName = "Abilities/Targeting/Self", order = 0)]
public class SelfTargeting : TargetingStrategy
{
    public override void Init(GameObject user)
    {
    }

    public override void StartTargeting(AbilityData data, Action finished)
    {
        data.Targets = new GameObject[] {data.User};
        data.TargetedPoint = data.User.transform.position;    
        finished();
    }
}
