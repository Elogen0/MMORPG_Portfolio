using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    
    [SerializeField] private LayerMask _layers = default;
    private List<Transform> innerObjects = new List<Transform>();
    private Transform target;
    
    private GameObject spawnText = null;

    private void Start()
    {
        ObjectSpawner.Instance.SpawnAsync(gameObject, InteractionText.Path, Vector3.zero, Quaternion.identity, null, "Interact [E]",
            go => { spawnText = go; go.SetActive(false);});
    }

    private void OnDestroy()
    {
        AddressableLoader.ReleaseInstance(spawnText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;
        if ((1 << other.gameObject.layer & _layers) != 0)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                if (!innerObjects.Contains(other.transform))
                {
                    innerObjects.Add(other.transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            return;
        if ((1 << other.gameObject.layer & _layers) != 0)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                innerObjects.Remove(other.transform);
            }
        }
    }

    private void Update()
    {
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        foreach (var t in innerObjects)
        {
            float distance = (t.position - transform.position).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = t;
            }
        }

        if (nearest != target)
        {
            if (spawnText)
                spawnText.SetActive(true);
                
            if (nearest && spawnText)
            {
                spawnText.transform.position = nearest.position + Vector3.up * 1f + Vector3.right * .5f;
            }
            else if (!nearest && spawnText)
            {
                spawnText.SetActive(false);
            }    
        }
        
        target = nearest;

        if (Input.GetKeyDown(KeyCode.E) && target)
        {
            if (target.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(this.transform.parent.gameObject);
            }
        }
    }
}
