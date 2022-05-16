using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities
{
    public abstract class TargetingStrategy : ScriptableObject
    {
        public abstract void Init(GameObject user);
        public abstract void StartTargeting(AbilityData data, Action finished);
    }
}
