using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour
{
    Animator anim;
    Transform hips, spine;
    Transform gunMuzzle;

    Vector3 initialRootRotation;
    Vector3 initialHipsRotation;
    Vector3 initialSpineRotation;

    NavMeshAgent navMesh;

    void Awake()
    {
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;


        foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            gunMuzzle = child.Find("muzzle");
            if (gunMuzzle != null)
            {
                break;
            }
            foreach (var member in GetComponentsInChildren<Rigidbody>())
            {
                member.isKinematic = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
