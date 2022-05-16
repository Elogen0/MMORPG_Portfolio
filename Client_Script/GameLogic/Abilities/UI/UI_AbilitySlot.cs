using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kame.Abilities.UI
{
    public class UI_AbilitySlot : MonoBehaviour
    {
        [SerializeField] Ability ability;
        [SerializeField] Image cooldownOverlay = null;

        CooldownStore cooldownStore;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            cooldownStore = player.GetComponent<CooldownStore>();
        }

        private void Update()
        {
            
            cooldownOverlay.fillAmount = cooldownStore.GetFractionRemaining(ability);
        }
    } 

    
}
