using System;
using System.Collections;
using System.Collections.Generic;
using Kame.AI;
using UnityEngine;

namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Delayed Effect", menuName = "Abilities/Effects/ChangeState", order = 0)]
    public class ChangeStateEffect : EffectStrategy
    {
        [SerializeField] StateSO state;
        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.User.GetComponent<StateController>()?.ChangeState(state);
        }
    }
}