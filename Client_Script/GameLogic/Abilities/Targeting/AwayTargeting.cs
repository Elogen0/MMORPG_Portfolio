using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using UnityEngine;

[CreateAssetMenu(fileName = "New Away Targeting", menuName = "Abilities/Targeting/AwayTargeting", order = 0)]
public class AwayTargeting : TargetingStrategy
{
    [SerializeField] private float forward;
    [SerializeField] private float right;
    [SerializeField] private float range;
    public override void Init(GameObject user)
    {
        
    }

    public override void StartTargeting(AbilityData data, Action finished)
    {
        Vector3 forward = data.User.transform.TransformDirection(Vector3.forward) * this.forward;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x) * this.right;

        Vector3 point = data.User.transform.position + forward + right;
        data.Targets = GetGameObjectsInRadius(point, range);
        data.TargetedPoint = point;
        finished();
    }
    
    private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point, float range)
    {
        RaycastHit[] hits = Physics.SphereCastAll(point, range, Vector3.up, 0);
        foreach (var hit in hits)
        {
            yield return hit.collider.gameObject;
        }
    }
}
