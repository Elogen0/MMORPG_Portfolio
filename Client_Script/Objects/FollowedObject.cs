using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollowedObject : MonoBehaviour, Spawnee
{
    private Transform follower;
    private Vector3 attachedPos;

    public void Setup(Transform follower, Vector3 attachedPos)
    {
        this.follower = follower;
        this.attachedPos = attachedPos;
    }

    protected void LateUpdate()
    {
        Reposition();
    }

    protected void Reposition()
    {
        if (!follower)
            return;
        transform.position = follower.position + attachedPos;
    }

    public abstract IEnumerator AfterSpawnAction(GameObject caller, object data);
}
