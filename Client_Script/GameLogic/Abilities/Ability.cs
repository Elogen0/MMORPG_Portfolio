using Kame.Abilities.Filters;
using Kame.Battles;
using Kame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kame.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IUsable
    {
        public string tag;
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float cooldownTime = 0;
        [SerializeField] float manaCost = 0;

        public void Init(GameObject user)
        {
            targetingStrategy.Init(user);
            foreach (var effectStrategy in effectStrategies)
            {
                effectStrategy.Init();
            }
        }

        public void Use(GameObject user)
        {
            if (user.TryGetComponent(out Mana mana))
            {
                if (mana.MP < manaCost)
                { return; }    
            }
            

            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0)
            {
                return;
            }

            AbilityData data = new AbilityData(user);
            user.GetComponent<ActionScheduler>().StartAction(data);
            targetingStrategy.StartTargeting(data, () => { TargetAcquired(data); });
        }

        private void TargetAcquired(AbilityData data)
        {
            if (data.IsCancelled()) return;

            if (data.User.TryGetComponent(out Mana mana) && !mana.UseMana(manaCost))
            {
                return;
            }

            CooldownStore cooldownStore = data.User.GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTime);

            foreach (var filterStrategy in filterStrategies)
            {
                data.Targets = filterStrategy.Filter(data.Targets);
            }
            

            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {

        }
    }
}
