using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Follow Prefab Effect", menuName = "Abilities/Effects/FollowPrefab", order = 0)]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] private Projectile protjectile;
        [SerializeField] private float damage;

        public override void Init()
        {
            
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            // Transform muzzle = data.User.GetComponent<StateController>().muzzle;
            //
            // Projectile instance = Instantiate(protjectile, muzzle.position, muzzle.rotation);
            // instance.damage = damage;
        }
    }
    
}
