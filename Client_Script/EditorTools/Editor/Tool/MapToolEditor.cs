using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[CustomEditor(typeof(MapTool))]
public class MapToolEditor : Editor
{
    private MapTool tool;
    public override void OnInspectorGUI()
    {
        tool = (MapTool) target;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        {
            if (GUILayout.Button("Generate Map", GUILayout.Width(150)))
            {
                tool.GenerateMap();
            }            
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (!tool)
            return;
        DrawToolBox(tool.start, tool.end);
        DrawAreaSpawn();
        //ShowExtractedMapGrid();
    }

    #region MapExtractBox
    Vector3 s1, s2, s3, s4;
    Vector3 e1, e2, e3, e4;

    private Vector3[] upperEdges = new Vector3[4];
    private Vector3[] bottomEdges = new Vector3[4];
    private void DrawToolBox(Vector3 start, Vector3 end)
    {
       
        upperEdges[0] = new Vector3(start.x, start.y, start.z);
        upperEdges[1] = new Vector3(end.x, start.y, start.z);
        upperEdges[2] = new Vector3(end.x, start.y, end.z);
        upperEdges[3] = new Vector3(start.x, start.y, end.z);
        
        bottomEdges[0] = new Vector3(start.x, end.y, start.z);
        bottomEdges[1] = new Vector3(end.x, end.y, start.z);
        bottomEdges[2] = new Vector3(end.x, end.y, end.z);
        bottomEdges[3] = new Vector3(start.x, end.y, end.z);
        // Handles.DrawSolidRectangleWithOutline(upperEdges, new Color(1,0,0 ,0.1f), new Color(1,0,0));
        // Handles.DrawSolidRectangleWithOutline(bottomEdges, new Color(1,0,0 ,0.1f), new Color(1,0,0));
        Handles.color = Color.red;
        for (int i = 0; i < upperEdges.Length; i++)
        {
            Handles.DrawLine(upperEdges[i % upperEdges.Length], upperEdges[(i+1) % upperEdges.Length]);
        }        

        Handles.DrawLine(upperEdges[0], bottomEdges[0]);
        Handles.DrawLine(upperEdges[1], bottomEdges[1]);
        Handles.DrawLine(upperEdges[2], bottomEdges[2]);
        Handles.DrawLine(upperEdges[3], bottomEdges[3]);
    }
    #endregion

    #region DrawAreaSpawn
    private void DrawAreaSpawn()
    {
        MapInfo info = tool.mapInfo;

        if (info.areaSpawns != null && info.areaSpawns.Length > 0)
        {
            foreach (var spawn in info.areaSpawns)
            {
                spawn.EditorColor.a = 0.4f;
                Handles.color = spawn.EditorColor;
                Handles.DrawSolidDisc(spawn.spawnPosition, Vector3.up, spawn.SpawnAreaRadius);
                spawn.EditorColor.a = 0.9f;
                // spawn.SpawnAreaRadius = Handles.ScaleSlider(spawn.SpawnAreaRadius, spawn.spawnPosition, Vector3.up, Quaternion.identity,
                //     spawn.SpawnAreaRadius, 0.1f);
                spawn.spawnPosition = Handles.PositionHandle(spawn.spawnPosition, Quaternion.identity);
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.fontSize = 15;
                labelStyle.normal.textColor = Color.black;
                labelStyle.alignment = TextAnchor.UpperCenter;
                Handles.Label(spawn.spawnPosition + Vector3.up * 3f, $"{spawn.Label}\n{spawn.MaxGenCount}" , labelStyle);
            }
        }
    }
    #endregion

    #region DrawExtractedMap
    Color validCellColor = new Color(0, 1f, 0, 1f);
    Color collisionCellColor = new Color(1f, 0, 0, 1f);
    private void ShowExtractedMapGrid()
    {
        if (tool.showExtracted)
        {
            foreach (var cell in tool.mapInfo.cellList)
            {
                Vector3[] verts = new Vector3[]
                {
                    new Vector3(cell.x - 0.5f, cell.h, cell.y - 0.5f),
                    new Vector3(cell.x - 0.5f, cell.h, cell.y + 0.5f),
                    new Vector3(cell.x + 0.5f, cell.h, cell.y + 0.5f),
                    new Vector3(cell.x + 0.5f, cell.h, cell.y - 0.5f)
                };
                Color cellColor = cell.collision ? collisionCellColor : validCellColor;
                Handles.color = cellColor;
                Handles.DrawSolidRectangleWithOutline(verts, cellColor, cellColor);
            }
        }
    }
    #endregion
    
    
    
    
    // private void DrawMapBounce()
    // {
    //     Handles.color = Color.red;
    //     tool.MapScaleX = Handles.ScaleSlider(tool.MapScaleX, tool.transform.position, Vector3.right, Quaternion.identity,
    //         tool.MapScaleX, 0.1f);
    //     Handles.color = Color.green;
    //     tool.MapScaleY = Handles.ScaleSlider(tool.MapScaleY, tool.transform.position, Vector3.up, Quaternion.identity,
    //         tool.MapScaleY, 0.1f);
    //     Handles.color = Color.blue;
    //     tool.MapScaleZ = Handles.ScaleSlider(tool.MapScaleZ, tool.transform.position, Vector3.forward, Quaternion.identity,
    //         tool.MapScaleZ, 0.1f);
    //     tool.MapScaleX = Mathf.Clamp(tool.MapScaleX, 2f, float.MaxValue);
    //     tool.MapScaleY = Mathf.Clamp(tool.MapScaleY, 2f, float.MaxValue);
    //     tool.MapScaleZ = Mathf.Clamp(tool.MapScaleZ, 2f, float.MaxValue);
    // }
}
