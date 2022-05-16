using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEffect : FollowedObject
{
    public override IEnumerator AfterSpawnAction(GameObject caller, object data)
    {
        if (caller.TryGetComponent(out Health health))
        {
            Setup(caller.transform, health.heartPosition);
        }
        else
        {
            Setup(caller.transform, transform.localPosition);
        }
        
        yield return null;
    }
}
