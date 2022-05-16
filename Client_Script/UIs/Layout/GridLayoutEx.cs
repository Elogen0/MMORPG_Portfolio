using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GridLayoutEx))]
public class GridLayoutExEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Fit Cell to Parent"))
        {
            (target as GridLayoutEx).CalculateCellSizeGridLayout();
        }
    }
}
#endif

[RequireComponent(typeof(GridLayoutGroup))]
//[ExecuteInEditMode]
public class GridLayoutEx : MonoBehaviour
{
    private void Start()
    {
        CalculateCellSizeGridLayout();
    }

    [ContextMenu("Calculate Cell Size")]
    public void CalculateCellSizeGridLayout()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        RectTransform trans = GetComponent<RectTransform>();
        Rect rect = trans.rect; 
        float width = rect.width;
        float height = rect.height;
        
        switch (grid.constraint)
        {
            case GridLayoutGroup.Constraint.FixedColumnCount :
                float horSpace = grid.padding.left + grid.padding.right + ((grid.constraintCount - 1) * grid.spacing.x);
                float calcChildWidth = (width - horSpace) / grid.constraintCount;
                grid.cellSize = new Vector2(calcChildWidth, grid.cellSize.y);
                foreach (RectTransform child in trans)
                {
                    child.sizeDelta = new Vector2(calcChildWidth, rect.width);
                }
                break;
            
            case GridLayoutGroup.Constraint.FixedRowCount :
                float vertSpace = grid.padding.top + grid.padding.bottom + ((grid.constraintCount - 1) * grid.spacing.y);
                float calcChildHeight = (height - vertSpace) / grid.constraintCount;
                grid.cellSize = new Vector2(grid.cellSize.x, calcChildHeight);
                break;
        }
    }
}
