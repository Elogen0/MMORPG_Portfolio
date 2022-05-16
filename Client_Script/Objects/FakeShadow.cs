using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeShadow : MonoBehaviour
{
    [Range(0, 0.5f)]
    [SerializeField] private float shadowHeight = 0.01f;

    [SerializeField] private float shadowSize = 0.2f;
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit[] hit = new RaycastHit[1];

    private void OnValidate()
    {
        transform.localScale = new Vector3(shadowSize, 1, shadowSize);
    }

    private void Awake()
    {
        transform.localScale = new Vector3(shadowSize, 1, shadowSize);
    }

    private void LateUpdate()
    {
        Debug.DrawLine(transform.parent.position + Vector3.up, transform.parent.position + Vector3.down * 5f, Color.magenta);

        if (Physics.RaycastNonAlloc(transform.parent.position + Vector3.up, Vector3.down, hit, 5f, raycastMask) > 0)
        {
            var position = this.transform.position;
            position = new Vector3(position.x, hit[0].point.y + shadowHeight, position.z);
            transform.position = position;
        }
    }
    
}
