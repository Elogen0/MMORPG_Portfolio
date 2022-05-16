using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public UnityEvent registerInteraction;
    public float Distance => Mathf.Infinity;
    public void Interact(GameObject other)
    {
        Debug.Log($"Interact{gameObject.name}");
        registerInteraction?.Invoke();
    }

    public void StopInteract(GameObject other)
    {
        
    }
}
