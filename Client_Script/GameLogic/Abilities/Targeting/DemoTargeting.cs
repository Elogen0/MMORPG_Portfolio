using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities
{
    [CreateAssetMenu(fileName = "New Targeting", menuName = "Abilities/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void Init(GameObject user)
        {
        }

        public override void StartTargeting(AbilityData data, Action finished)
        {
            Debug.Log("Demo Targeting Started");
            finished();
        }
    }

}
