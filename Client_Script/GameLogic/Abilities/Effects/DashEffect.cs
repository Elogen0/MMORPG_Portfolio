using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using UnityEngine;

public class DashEffect : EffectStrategy
{
    [SerializeField] private float duration;
    [SerializeField] private float speed;

    public override void Init()
    {
        
    }

    public override void StartEffect(AbilityData data, Action finished)
    {
    }
}
