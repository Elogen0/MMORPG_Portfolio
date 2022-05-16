using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_FollowedObject : UI_Base, Spawnee
{
    protected RectTransform _rect;
    protected Transform follower;
    protected Vector3 attachedPos;

    public override void Init()
    {
        _rect = GetComponent<RectTransform>();
    }

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
        _rect.position = follower.position + attachedPos;
    }

    public abstract IEnumerator AfterSpawnAction(GameObject caller, object data);
}
