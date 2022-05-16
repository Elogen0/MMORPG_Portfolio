using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> VisibleTargets => visibleTargets;

    private Transform nearestTarget;
    public Transform NearestTarget => nearestTarget;
    private float distanceToTarget = 0.0f;

    public float delay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", delay);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    void FindVisibleTargets()
    {
        distanceToTarget = 0.0f;
        nearestTarget = null;
        visibleTargets.Clear();

        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetInViewRadius.Length; ++i)
        {
            Transform target = targetInViewRadius[i].transform;

            IDamagable damagable = target.GetComponentInParent<IDamagable>();
            if (damagable == null)
                continue;
            if (damagable.IsDead)
                continue;

            Vector3 dirTorTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirTorTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirTorTarget, dstToTarget, obstacleMask)) //장애물이 없다면
                {
                    visibleTargets.Add(target);
                    if (nearestTarget == null || (distanceToTarget > dstToTarget))
                    {
                        nearestTarget = target;
                        distanceToTarget = dstToTarget;
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}