using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Kame.Sounds;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// 발자국 소리를 출력.
/// </summary>
public class PlayerFootStep : MonoBehaviour
{
    public SoundList[] stepSounds;
    private Animator myAnimator;
    private int index;
    private Transform leftFoot, rightFoot;
    private float liftedFootPosY;
    private int groundedBool, coverBool, aimBool, crouchFloat;
    private bool grounded;
    private CharacterController _controller;
    public enum Foot
    {
        LEFT,
        RIGHT,
    }
    private Foot step = Foot.LEFT;
    private float prevOffsetY, highestPosY = 0;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        leftFoot = myAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = myAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
    }
    private void PlayFootStep()
    {
        if(prevOffsetY < highestPosY) // 발이 올라가는 중이면
            return;
        
        prevOffsetY = highestPosY = 0;
        int oldIndex = index;
        while(oldIndex == index)
        {
            index = Random.Range(0, stepSounds.Length - 1);
        }
        Debug.Log("FootStepSound");
        SoundManager.Instance.Play(stepSounds[index], transform.position);
    }
    private void Update()
    {
        if(!grounded && _controller.isGrounded)
        {
            PlayFootStep();
        }

        grounded = _controller.isGrounded;
        float factor = 0.2f;

        if(grounded && _controller.velocity.magnitude > 1f)
        {
            prevOffsetY = highestPosY;
             switch(step)
             {
                 case Foot.LEFT:
                     liftedFootPosY = leftFoot.position.y - transform.position.y;
                     
                     highestPosY = liftedFootPosY > highestPosY ? liftedFootPosY : highestPosY;
                     if(liftedFootPosY <= factor)
                     {
                         Debug.Log("PlayFootstep");

                         PlayFootStep();
                         step = Foot.RIGHT;
                     }
                     break;
                 case Foot.RIGHT:
                     liftedFootPosY = rightFoot.position.y - transform.position.y;
                     highestPosY = liftedFootPosY > highestPosY ? liftedFootPosY : highestPosY;
                     if(liftedFootPosY <= factor)
                     {
                         Debug.Log("PlayFootstep");
                         PlayFootStep();
                         step = Foot.LEFT;
                     }
                     break;
             }
        }
    }


}
