using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Define;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

[System.Serializable]
public class CursorMapping
{
    public CursorType type;
    public Texture2D texture;
    public Vector2 hotspot;
}

public class CursorManager : SingletonMono<CursorManager>
{
#region Inspector
    [SerializeField] private float spherecastRadius = 0.1f;
    [SerializeField] float maxNavMeshProjectionDistance = 1f;
    [SerializeField] private float maxNavPathLength = 40f;
    [Space(2)] 
    [SerializeField] PlaceTargetPointer TargetPointer;
    #endregion

    #region Properties
    private CursorMapping[] CursorMappings => _settings.cursorMappings;
    public Vector3 WorldPosition { get ; set; } = Vector3.negativeInfinity;
    public Transform NearestObject { get; private set; }= null;
    #endregion
    
#region Variables
    private const int MAX_DETECT_COUNT = 5;
    
    private Camera _mainCamera;
    private Ray _mouseRay;
    private GameObject _player;
    private bool _isDraggingUI = false;
    private PlaceTargetPointer _pointer;
    private CursorSettings _settings;
#endregion

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        _settings = CursorSettings.Get();
        _mainCamera = Camera.main;
        _player = GameObject.FindWithTag(TagAndLayer.TagName.Player);
        _pointer = Instantiate(TargetPointer, transform);
    }

    void Start()
    { 
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
         if (CursorMappings.Length > 0)
         {
             Cursor.SetCursor(CursorMappings[0].texture, CursorMappings[0].hotspot, CursorMode.Auto);
         }
         else
         {
             Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
         }
    }

    private void Update()
    {
        _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (InteractWithUI()) return;
        if (!_player)
        {
            _player = GameObject.FindWithTag(TagAndLayer.TagName.Player);
            return;
        }
        
        if (InteractWithComponent()) return;
        //if (InteractWithNavMesh()) return;
        SetCursor(CursorType.None);
    }
    #endregion
    
    private bool InteractWithUI()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingUI = false;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Interact With UI" + EventSystem.current.name);
                _isDraggingUI = true;
            }
            SetCursor(CursorType.UI);
            return true;
        }
        
        if (_isDraggingUI)
        {
            return true;
        }
        return false;
    }

    private bool InteractWithComponent()
    {
        //DetectObject
        //RaycastHit[] hits = Physics.SphereCastAll(_mouseRay, spherecastRadius);
        RaycastHit[] hits = Physics.RaycastAll(_mouseRay);
        if (hits.Length <= 0)
        {
            NearestObject = null;
            return false;
        }
        
        Array.Sort(hits, (a, b) =>
        {
            if (a.distance > b.distance)
                return 1;
            else if (a.distance < b.distance)
                return -1;
            else
                return 0;
        });

        NearestObject = hits[0].transform;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (NearestObject.TryGetComponent(out Health health))
            {
                _pointer.SetTarget(NearestObject);
                //_player.GetComponent<PlayerController>().SetTarget(NearestObject);
            }
            
            if (NearestObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(_player);
            }

            if (NearestObject.TryGetComponent(out CursorInteractor cursoredObject))
            {
                cursoredObject.OnClick?.Invoke();
            }
        }

        //SetCursor
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].transform.TryGetComponent(out CursorInteractor cursoredObject))
            {
                if (cursoredObject.active)
                {
                    SetCursor(cursoredObject.CurrentCursorType);
                    return true;
                }
            }
        }
        
        return false;
    }

    #region NavMesh
    private bool InteractWithNavMesh()
    {
        if (RaycastNavMesh(out Vector3 hitPoint, out RaycastHit hit))
        {
            if (!CanMoveTo(hitPoint))
            {
                SetCursor(CursorType.CantMove);
                return true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                _pointer.SetPosition(hit);
                //_player.GetComponent<PlayerController>().MoveAgent(hitPoint);
                //MoveMent
            }
            SetCursor(CursorType.Movement);
            return true;
        }
        return false;
    }

    private bool RaycastNavMesh(out Vector3 hitPoint, out RaycastHit hit)
    {
        hitPoint = Vector3.negativeInfinity;

        if (!Physics.Raycast(_mouseRay, out hit))
            return false;

        if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas))
            return false;

        hitPoint = navMeshHit.position;
        return true;
    }
    
    public bool CanMoveTo(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(_player.transform.position, destination, NavMesh.AllAreas, path))
            return false;
        
        if (path.status != NavMeshPathStatus.PathComplete)
            return false;
        
        if (GetCornerLength(path) > maxNavPathLength)
            return false;
        return true;
    }
    
    private float GetCornerLength(NavMeshPath path)
    {
        float total = 0;
        if (path.corners.Length < 2) return 0;

        for (int i = 0; i < path.corners.Length -1; ++i)
        {
            total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
            
        return total;
    }
    #endregion
    
    
    private void SetCursor(CursorType type)
    {
        CursorMapping mapping = GetCursorMapping(type);
        Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCursorMapping(CursorType type)
    {
        foreach (CursorMapping mapping in CursorMappings)
        {
            if (mapping.type == type)
            {
                return mapping;
            }
        }

        return CursorMappings[0];
    }
}
