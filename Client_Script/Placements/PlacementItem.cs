using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Utilities;
using UnityEngine;

public class PlacementItem : MonoBehaviour
{
    public string itemName;
    public IntVector2 dimensions;
    public PlacementGhost ghostPrefab;
    public IntVector2 gridPosition { get; private set; }
    public IPlacementArea placementArea { get; private set; }
    public Action OnDeletedAction;
    public Action OnDestroyedAction;
    
    public virtual void Initialize(IPlacementArea targetArea, IntVector2 destination)
    {
        placementArea = targetArea;
        gridPosition = destination;

        if (targetArea != null)
        {
            transform.position = placementArea.GridToWorld(destination, dimensions);
            transform.rotation = placementArea.transform.rotation;
            targetArea.Occupy(destination, dimensions);
        }
    }
    
    /// <summary>
    /// Removes tower from placement area and destroys it
    /// </summary>
    public void Remove()
    {
        placementArea.Clear(gridPosition, dimensions);
        Destroy(gameObject);
    }
}