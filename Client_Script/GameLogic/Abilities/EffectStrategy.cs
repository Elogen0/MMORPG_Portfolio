using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities
{
    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void Init();
        public abstract void StartEffect(AbilityData data, Action finished);
    } 
}
