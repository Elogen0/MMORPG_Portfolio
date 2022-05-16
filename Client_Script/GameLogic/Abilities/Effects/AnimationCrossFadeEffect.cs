using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Trigger Animation Effect", menuName = "Abilities/Effects/TriggerAnimation", order = 0)]
    public class AnimationCrossFadeEffect : EffectStrategy
    {
        [SerializeField] private string animationName;
        [SerializeField] private float fadeTime = 0.2f;
        [SerializeField] private int layer = 0;
        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            Animator animator = data.User.GetComponent<Animator>();
            animator.CrossFade(animationName, fadeTime, layer);
            finished();
        }
    }
}