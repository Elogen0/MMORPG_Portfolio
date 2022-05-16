using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGroup : MonoBehaviour
{
    public Transform[] stations;

#if UNITY_EDITOR
    [SerializeField] private bool drawGizmo = true;
    public void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            for (int i = 0; i < stations.Length; ++i)
            {
                Vector3 pos = stations[i].position;
                Gizmos.DrawSphere(stations[i].position, 0.5f);
            }
        }
    }
#endif
}
