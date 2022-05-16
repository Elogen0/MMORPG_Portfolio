using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using UnityEngine;
[CreateAssetMenu(fileName = "New Null Targeting", menuName = "Abilities/Targeting/Null", order = 0)]
public class NullTargeting : TargetingStrategy
{
    public override void Init(GameObject user)
    {
    }

    public override void StartTargeting(AbilityData data, Action finished)
    {
        finished();
    }
}
