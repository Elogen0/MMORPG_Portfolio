using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Orient Target Effect", menuName = "Abilities/Effects/OrientToTarget", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {
        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.User.transform.LookAt(data.TargetedPoint);
        }
    }

}