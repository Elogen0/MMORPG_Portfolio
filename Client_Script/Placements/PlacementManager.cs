using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

public struct UIPointer
{
    public PointerInfo pointer;
    public Ray ray;
    public RaycastHit? raycast;
    public bool overUI;
}

[RequireComponent(typeof(Camera))]
public class PlacementManager : SceneSingletonMono<PlacementManager>
{
    public enum BuildState
    {
        Normal,
        Building,
        Paused,
        GameOver,
        BuildingWithDrag
    }

    public BuildState buildState { get; private set; }
    public LayerMask placementAreaMask;
    public LayerMask itemSelectionLayer;
    public LayerMask ghostWorldPlacementMask;
    public float sphereCastRadius = 1;

    public event Action<BuildState, BuildState> stateChanged;
    public event Action ghostBecameValid;
    public event Action<PlacementItem> selectionChanged;
    
    IPlacementArea m_CurrentArea;
    IntVector2 m_GridPosition;
    Camera m_Camera;
    PlacementGhost currentGhost;
    bool m_GhostPlacementPossible;
    public PlacementItem CurrentSelectedPlacementItem { get; private set; }
    public bool isTowerSelected => CurrentSelectedPlacementItem != null;

    public bool isBuilding => (buildState == BuildState.Building || buildState == BuildState.BuildingWithDrag);

    public void CancelGhostPlacement()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Can't cancel out of ghost placement when not in the building state.");
        }

        Destroy(currentGhost.gameObject);
        currentGhost = null;
        ChangeBuildState(BuildState.Normal);
        DeselectItem();
    }


    public void ChangeToDragMode()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to return to Build With Dragging Mode when not in Build Mode");
        }

        ChangeBuildState(BuildState.BuildingWithDrag);
    }

    public void ReturnToBuildMode()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to return to Build Mode when not in Drag Mode");
        }

        ChangeBuildState(BuildState.Building);
    }

    void ChangeBuildState(BuildState newBuildState)
    {
        if (buildState == newBuildState)
        {
            return;
        }
        OnStateExit(buildState);
        OnStateEnter(buildState, newBuildState);
    }

    private void OnStateExit(BuildState oldBuildState)
    {
        if (oldBuildState == BuildState.Paused || oldBuildState == BuildState.GameOver)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnStateEnter(BuildState oldBuildState, BuildState newBuildState)
    {
        switch (newBuildState)
        {
            case BuildState.Normal:
                break;
            case BuildState.Building:
                break;
            case BuildState.BuildingWithDrag:
                break;
            case BuildState.Paused:
            case BuildState.GameOver:
                if (oldBuildState == BuildState.Building)
                {
                    CancelGhostPlacement();
                }

                Time.timeScale = 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newBuildState, null);
        }

        buildState = newBuildState;
        
        if (stateChanged != null)
        {
            stateChanged(oldBuildState, buildState);
        }
    }
    

    public void GameOver()
    {
        ChangeBuildState(BuildState.GameOver);
    }


    public void Pause()
    {
        ChangeBuildState(BuildState.Paused);
    }

    public void Unpause()
    {
        ChangeBuildState(BuildState.Normal);
    }

    public void SetToDragMode([NotNull] PlacementItem placementItemToBuild)
    {
        if (buildState != BuildState.Normal)
        {
            throw new InvalidOperationException("Trying to enter drag mode when not in Normal mode");
        }

        if (currentGhost != null)
        {
            // Destroy current ghost
            CancelGhostPlacement();
        }

        SetUpGhostTower(placementItemToBuild);
        ChangeBuildState(BuildState.BuildingWithDrag);
    }

    public void SetToBuildMode([NotNull] PlacementItem placementItemToBuild)
    {
        if (buildState != BuildState.Normal)
        {
            throw new InvalidOperationException("Trying to enter Build mode when not in Normal mode");
        }

        if (currentGhost != null)
        {
            Debug.Log("CancelTower");
            // Destroy current ghost
            CancelGhostPlacement();
        }

        SetUpGhostTower(placementItemToBuild);
        ChangeBuildState(BuildState.Building);
    }

    public void TryPlaceTower(PointerInfo pointerInfo)
    {
        UIPointer pointer = WrapPointer(pointerInfo);

        // Do nothing if we're over UI
        if (pointer.overUI)
        {
            return;
        }

        PutItem(pointer);
    }

    public void TryMoveGhost(PointerInfo pointerInfo, bool hideWhenInvalid = true)
    {
        if (currentGhost == null)
        {
            throw new InvalidOperationException("Trying to move the tower ghost when we don't have one");
        }

        UIPointer pointer = WrapPointer(pointerInfo);
        // Do nothing if we're over UI
        if (pointer.overUI && hideWhenInvalid)
        {
            currentGhost.Hide();
            return;
        }

        MoveGhost(pointer, hideWhenInvalid);
    }

    public void SelectItem(PlacementItem placementItem)
    {
        if (buildState != BuildState.Normal)
        {
            throw new InvalidOperationException("Trying to select whilst not in a normal state");
        }

        DeselectItem();
        CurrentSelectedPlacementItem = placementItem;

        if (selectionChanged != null)
        {
            selectionChanged(placementItem);
        }
    }
    
    public void PutItem()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to buy towers when not in Build Mode");
        }
        if (currentGhost == null || !IsGhostAtValidPosition())
        {
            return;
        }
        
        PlaceItem();
    }

    public void PutItem(UIPointer pointer)
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to buy towers when not in a Build Mode");
        }

        if (currentGhost == null || !IsGhostAtValidPosition())
        {
            return;
        }

        PlacementAreaRaycast(ref pointer);
        if (!pointer.raycast.HasValue || pointer.raycast.Value.collider == null)
        {
            CancelGhostPlacement();
            return;
        }

        PlaceGhost(pointer);
    }


    public void DeselectItem()
    {
        if (buildState != BuildState.Normal)
        {
            throw new InvalidOperationException("Trying to deselect tower whilst not in Normal state");
        }

        CurrentSelectedPlacementItem = null;

        selectionChanged?.Invoke(null);
    }

    public bool IsGhostAtValidPosition()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to check ghost position when not in a build mode");
        }

        if (currentGhost == null)
        {
            return false;
        }

        if (m_CurrentArea == null)
        {
            return false;
        }

        PlacementFitStatus fits = m_CurrentArea.Fits(m_GridPosition, currentGhost.mainBody.dimensions);
        return fits == PlacementFitStatus.Fits;
    }

    public void PlaceItem()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to place tower when not in a Build Mode");
        }

        if (!IsGhostAtValidPosition())
        {
            throw new InvalidOperationException("Trying to place tower on an invalid area");
        }

        if (m_CurrentArea == null)
        {
            return;
        }

        PlacementItem createdPlacementItem = Instantiate(currentGhost.mainBody);
        createdPlacementItem.Initialize(m_CurrentArea, m_GridPosition);

        CancelGhostPlacement();
    }

    public bool IsPointerOverGhost(PointerInfo pointerInfo)
    {
        if (buildState != BuildState.Building)
        {
            throw new InvalidOperationException("Trying to tap on ghost tower when not in Build Mode");
        }

        UIPointer uiPointer = WrapPointer(pointerInfo);
        RaycastHit hit;
        return currentGhost.ghostCollider.Raycast(uiPointer.ray, out hit, float.MaxValue);
    }

    public void TrySelectItem(PointerInfo info)
    {
        if (buildState != BuildState.Normal)
        {
            throw new InvalidOperationException("Trying to select towers outside of Normal state");
        }

        UIPointer uiPointer = WrapPointer(info);
        RaycastHit output;
        bool hasHit = Physics.Raycast(uiPointer.ray, out output, float.MaxValue, itemSelectionLayer);
        if (!hasHit || uiPointer.overUI)
        {
            return;
        }

        var controller = output.collider.GetComponent<PlacementItem>();
        if (controller != null)
        {
            SelectItem(controller);
        }
    }

    public Vector3 GetGhostPosition()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to get ghost position when not in a Build Mode");
        }

        if (currentGhost == null)
        {
            throw new InvalidOperationException("Trying to get ghost position for an object that does not exist");
        }

        return currentGhost.transform.position;
    }

    public void MoveGhostToCenter()
    {
        if (buildState != BuildState.Building)
        {
            throw new InvalidOperationException("Trying to move ghost when not in Build Mode");
        }

        // try to find a valid placement 
        Ray ray = m_Camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit placementHit;

        if (Physics.SphereCast(ray, sphereCastRadius, out placementHit, float.MaxValue, placementAreaMask))
        {
            MoveGhostWithRaycastHit(placementHit);
        }
        else
        {
            MoveGhostOntoWorld(ray, false);
        }
    }
    
    protected override void Awake()
    {
        base.Awake();

        buildState = BuildState.Normal;
        m_Camera = GetComponent<Camera>();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Time.timeScale = 1f;
    }

    protected UIPointer WrapPointer(PointerInfo pointerInfo)
    {
        return new UIPointer
        {
            overUI = IsOverUI(pointerInfo),
            pointer = pointerInfo,
            ray = m_Camera.ScreenPointToRay(pointerInfo.currentPosition)
        };
    }
    
    protected bool IsOverUI(PointerInfo pointerInfo)
    {
        int pointerId;
        EventSystem currentEventSystem = EventSystem.current;

        // Pointer id is negative for mouse, positive for touch
        var cursorInfo = pointerInfo as MouseCursorInfo;
        var mbInfo = pointerInfo as MouseButtonInfo;
        var touchInfo = pointerInfo as TouchInfo;

        if (cursorInfo != null)
        {
            pointerId = PointerInputModule.kMouseLeftId;
        }
        else if (mbInfo != null)
        {
            // LMB is 0, but kMouseLeftID = -1;
            pointerId = -mbInfo.mouseButtonId - 1;
        }
        else if (touchInfo != null)
        {
            pointerId = touchInfo.touchId;
        }
        else
        {
            throw new ArgumentException("Passed pointerInfo is not a TouchInfo or MouseCursorInfo", "pointerInfo");
        }

        return currentEventSystem.IsPointerOverGameObject(pointerId);
    }
    
    protected void MoveGhost(UIPointer pointer, bool hideWhenInvalid = true)
    {
        if (currentGhost == null || !isBuilding)
        {
            throw new InvalidOperationException(
                "Trying to position a tower ghost while the UI is not currently in the building state.");
        }

        // Raycast onto placement layer
        PlacementAreaRaycast(ref pointer);

        if (pointer.raycast != null)
        {
            MoveGhostWithRaycastHit(pointer.raycast.Value);
        }
        else
        {
            MoveGhostOntoWorld(pointer.ray, hideWhenInvalid);
        }
    }

    
    protected virtual void MoveGhostWithRaycastHit(RaycastHit raycast)
    {
        // We successfully hit one of our placement areas
        // Try and get a placement area on the object we hit
        m_CurrentArea = raycast.collider.GetComponent<IPlacementArea>();

        if (m_CurrentArea == null)
        {
            Debug.LogError("There is not an IPlacementArea attached to the collider found on the m_PlacementAreaMask");
            return;
        }

        m_GridPosition = m_CurrentArea.WorldToGrid(raycast.point, currentGhost.mainBody.dimensions);
        PlacementFitStatus fits = m_CurrentArea.Fits(m_GridPosition, currentGhost.mainBody.dimensions);

        currentGhost.Show();
        m_GhostPlacementPossible = fits == PlacementFitStatus.Fits;
        currentGhost.Move(m_CurrentArea.GridToWorld(m_GridPosition, currentGhost.mainBody.dimensions),
            m_CurrentArea.transform.rotation,
            m_GhostPlacementPossible);
    }
    
    protected virtual void MoveGhostOntoWorld(Ray ray, bool hideWhenInvalid)
    {
        m_CurrentArea = null;

        if (!hideWhenInvalid)
        {
            RaycastHit hit;
            // check against all layers that the ghost can be on
            Physics.SphereCast(ray, sphereCastRadius, out hit, float.MaxValue, ghostWorldPlacementMask);
            if (hit.collider == null)
            {
                return;
            }

            currentGhost.Show();
            currentGhost.Move(hit.point, hit.collider.transform.rotation, false);
        }
        else
        {
            currentGhost.Hide();
        }
    }
    
    protected void PlaceGhost(UIPointer pointer)
    {
        if (currentGhost == null || !isBuilding)
        {
            throw new InvalidOperationException(
                "Trying to position a tower ghost while the UI is not currently in a building state.");
        }

        MoveGhost(pointer);

        if (m_CurrentArea != null)
        {
            PlacementFitStatus fits = m_CurrentArea.Fits(m_GridPosition, currentGhost.mainBody.dimensions);

            if (fits == PlacementFitStatus.Fits)
            {
                // Place the ghost
                PlacementItem controller = currentGhost.mainBody;

                PlacementItem createdPlacementItem = Instantiate(controller);
                createdPlacementItem.Initialize(m_CurrentArea, m_GridPosition);

                CancelGhostPlacement();
            }
        }
    }
    
    protected void PlacementAreaRaycast(ref UIPointer pointer)
    {
        pointer.raycast = null;

        if (pointer.overUI)
        {
            // Pointer is over UI, so no valid position
            return;
        }

        // Raycast onto placement area layer
        RaycastHit hit;
        if (Physics.Raycast(pointer.ray, out hit, float.MaxValue, placementAreaMask))
        {
            pointer.raycast = hit;
        }
    }

    void SetUpGhostTower([NotNull] PlacementItem placementItemToBuild)
    {
        if (placementItemToBuild == null)
        {
            throw new ArgumentNullException("placementItemToBuild");
        }

        currentGhost = Instantiate(placementItemToBuild.ghostPrefab);
        currentGhost.Initialize(placementItemToBuild);
        currentGhost.Hide();
    }
}