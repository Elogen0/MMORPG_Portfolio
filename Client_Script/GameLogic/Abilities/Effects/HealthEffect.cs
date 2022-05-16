using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New HealthEffect", menuName = "Abilities/Effects/Health", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] private int healthChange;

        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (var target in data.Targets)
            {
                var health = target.GetComponent<Health>();
                if (health)
                {
                    if (healthChange < 0)
                    {
                        health.TakeDamage(data.User, -healthChange);
                    }
                    else
                    {
                        health.Heal(data.User, healthChange);
                    }
                }
            }
        }
    }

}