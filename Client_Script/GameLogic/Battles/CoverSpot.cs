using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.U2D;

public class CoverSpot : MonoBehaviour
{
    public Transform[] hidePoint;

    static List<CoverSpot> allCoverSpot = new List<CoverSpot>();
    public bool IsOccupied { get; set; } = false;
    public bool IsDestroyed { get; set; } = false;


    private void OnEnable()
    {
        if (!allCoverSpot.Contains(this))
        {
            allCoverSpot.Add(this);
        }
    }
    private void OnDisable()
    {
        if (allCoverSpot.Contains(this))
        {
            allCoverSpot.Remove(this);
        }
    }

    public Transform GetHidePoint(Transform target)
    {
        float dst = 0;
        Transform result = null;
        foreach (var hidePoint in this.hidePoint)
        {
            float sqrMgn = Vector3.SqrMagnitude(hidePoint.position - target.position);
            if (result == null || dst < sqrMgn)
            {
                result = hidePoint;
                dst = sqrMgn;
            }
        }

        return result;
    }

    public void GetIn()
    {
        IsOccupied = true;
    }

    public void GetOut()
    {
        IsOccupied = false;
    }

    public static CoverSpot GetBestCover(Transform self, Transform target, float inRanage)
    {
        CoverSpot nearestCover = null;
        float distanceToCover = 0.0f;

        foreach (var cover in SearchCoverInRange(self, target, inRanage))
        {
            float sqrMgn = Vector3.SqrMagnitude(cover.transform.position - self.position);
            
            if (nearestCover == null || (distanceToCover > sqrMgn))
            {
                nearestCover = cover;
                distanceToCover = sqrMgn;
            }
        }

        return nearestCover;
    }

    private static IEnumerable<CoverSpot> SearchCoverInRange(Transform self, Transform target, float inRange)
    {
        foreach (var spot in allCoverSpot)
        {
            if (spot.IsOccupied || spot.IsDestroyed)
            {
                continue;
            }

            float sqrMgn = Vector3.SqrMagnitude(spot.transform.position - target.position);
            if (sqrMgn > inRange * inRange)
                continue;

            Vector3 toTargetLine = target.position - spot.transform.position;
            Vector3 toSelfLine = self.position - spot.transform.position;
            if (Vector3.Angle(toTargetLine, toSelfLine) < 90f)
            {
                continue;
            }

            bool isValidCover = true;
            foreach (var hidepoint in spot.hidePoint)
            {
                if (Vector3.SqrMagnitude(hidepoint.position - target.position) > inRange * inRange)
                {
                    isValidCover = false;
                }
            }

            if (isValidCover)
            {
                yield return spot;
            }
        }
    }
}
