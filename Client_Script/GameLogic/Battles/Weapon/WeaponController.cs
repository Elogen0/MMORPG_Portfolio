using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using TMPro;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Transform armedWeapon;
    private Transform equippedWeapon;
    private Animator _anim;

    private bool _isInBattle = false;
    private ParticleSystem slashTrail;

    private int attackHash = Animator.StringToHash("Base Layer.attack");

    private void Start()
    {
        _anim = GetComponent<Animator>();
        equippedWeapon = _anim.GetBoneTransform(HumanBodyBones.Chest).Find("B_Slot");
        armedWeapon = _anim.GetBoneTransform(HumanBodyBones.RightHand).Find("R_Slot");
        if (armedWeapon)
            armedWeapon.gameObject.SetActive(false);
        if (slashTrail)
        {
            slashTrail = armedWeapon.GetComponentsInChildren<ParticleSystem>()[0];
            slashTrail.gameObject.SetActive(false);    
        }
    }

    private void Update()
    {
        if (!_anim)
            return;
        
        //int animatorHash = _anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        if (_anim.GetBool(AnimatorHash.IsAttacking))
        {
            if (slashTrail)
                slashTrail.gameObject.SetActive(true);
            _isInBattle = true;
            ToggleWeapon(_isInBattle);
        }
        else if (!_anim.GetBool(AnimatorHash.IsAttacking))
        {
            if (slashTrail)
                slashTrail.gameObject.SetActive(false);
            _isInBattle = false;
            ToggleWeapon(_isInBattle);
        }
    }

    private void ToggleWeapon(bool armed)
    {
        if (!armedWeapon || !equippedWeapon)
            return;
        armedWeapon.gameObject.SetActive(armed);
        equippedWeapon.gameObject.SetActive(!armed);
        
        // if (armed == false)
        //     AddressableLoader.InstantiateAsync("Assets/Game/Prefab/Effect/weapon disapear.prefab", armedWeapon.transform.position + armedWeapon.transform.TransformDirection(Vector3.up* 0.5f), armedWeapon.rotation);
    }

}
