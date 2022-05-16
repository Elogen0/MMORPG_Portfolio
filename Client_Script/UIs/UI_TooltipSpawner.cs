using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public abstract class UI_TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("The prefab of the tooltip to spawn")] [SerializeField]
    private AssetReference _tooltipPrefab = null;

    private GameObject _tooltip = null;
    private Canvas _parentCanvas;

    /// <summary>
    /// Called when it is time to update the information on the tooltip
    /// prefab.
    /// </summary>
    /// <param name="tooltip">
    /// The spawned tooltip prefab for updating.
    /// </param>
    public abstract void UpdateTooltip(GameObject tooltip);

    /// <summary>
    /// Return true when the tooltip spawner should be allowed to create a tooltip.
    /// </summary>
    public abstract bool CanCreateTooltip();

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    IEnumerator Start()
    {
        var request = AddressableLoader.InstantiatePooling(_tooltipPrefab, _parentCanvas.transform);
        yield return request.Wait();
        _tooltip = request.Result;
        _tooltip.SetActive(false);
    }

    private void OnDestroy()
    {
        //ClearTooltip();
    }

    private void OnDisable()
    {
        //ClearTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltip && !CanCreateTooltip())
        {
            ClearTooltip();
        }
        //
        // if (!_tooltip && CanCreateTooltip())
        // {
        //     _tooltip = Instantiate(tooltipPrefab, _parentCanvas.transform);
        // }

        if (_tooltip && CanCreateTooltip())
        {
            UpdateTooltip(_tooltip);
            PositionTooltip();
            _tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //_tooltip.SetActive(false);
        ClearTooltip();
    }

    private void PositionTooltip()
    {
        Canvas.ForceUpdateCanvases();

        var tooltipCorners = new Vector3[4];
        _tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);
        var slotCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(slotCorners);

        bool below = transform.position.y > Screen.height / 2;
        bool right = transform.position.x < Screen.width / 2;

        //getCordner
        int slotCorner = GetCornerIndex(below, right);
        int tooltipCorner = GetCornerIndex(!below, !right);

        _tooltip.transform.position =
            slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + _tooltip.transform.position;
    }

    private int GetCornerIndex(bool below, bool right)
    {
        //12
        //03
        if (below && !right) return 0;
        if (!below && !right) return 1;
        if (!below && right) return 2;
        return 3;
    }

    private void ClearTooltip()
    {
        _tooltip.SetActive(false);
        // if (_tooltip)
        // {
        //     Destroy(_tooltip.gameObject);
        //     _tooltip = null;
        // }
    }
}