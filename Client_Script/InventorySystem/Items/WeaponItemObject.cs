using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Game.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Kame/InventorySystem/WeaponItem", order = 2)]
public class WeaponItemObject : EquipItemObject
{
    [SerializeField] private AnimatorOverrideController animatorOverride = null;

    public bool ApplyAnimatorOverride(Animator animator)
    {
        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (animatorOverride != null)
        {
            animator.runtimeAnimatorController = animatorOverride;
            return true;
        }
        
        if (overrideController != null)
        {
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }
        return false;
    }

    public void ChangeAnimatorController(Animator animator, string AnimatorPath)
    {
        AddressableLoader.LoadAssetAsync<RuntimeAnimatorController>($"Assets/Animation/{AnimatorPath}", controller =>
        {
            if (controller == null)
                return;

            animator.runtimeAnimatorController = controller;
        });
    }

    public override void Use(Item item, GameObject User)
    { 
        base.Use(item, User);
        Animator animator = User.GetComponent<Animator>();
        if (!animator)
            return;
    }
}
