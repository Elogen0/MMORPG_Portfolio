using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Kame;
using UnityEditor;
using UnityEngine;

public class MapTool : MonoBehaviour
{
    public Vector3 MapScale = Vector3.one;
    public bool showExtracted = true;
    [Header("Map Extract Settings")]
    public int resolution = 1;
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public string outputName;

    public Vector3 start => 
        new Vector3((int)(transform.position.x + MapScale.x), (int)(transform.position.y + MapScale.y), (int)(transform.position.z + MapScale.z));
    public Vector3 end => 
        new Vector3((int)(transform.position.x - MapScale.x), (int)(transform.position.y - MapScale.y), (int)(transform.position.z - MapScale.z));
    
    public MapInfo mapInfo = new MapInfo();

    [ContextMenu("extract Vertex")]
    public void GenerateMap()
    {
        ExtractVertex("Resources/Map");
        ExtractVertex("../../Common/MapData");
    }

    public void ExtractVertex(string pathPrefix)
    {
        int xMin = int.MaxValue;
        int xMax = int.MinValue;
        int zMin = int.MaxValue;
        int zMax = int.MinValue;

        for (float x = Mathf.Min(start.x, end.x); x < Mathf.Max(start.x, end.x); x += resolution)
        {
            for (float z = Mathf.Min(start.z, end.z); z < Mathf.Max(start.z, end.z); z += resolution)
            {
                if (Physics.Raycast(new Vector3(x, start.y, z),
                    Vector3.down,
                    out RaycastHit hit,
                    Math.Abs(start.y - end.y),
                    groundMask | obstacleMask))
                {
                    if (hit.point.x < xMin) xMin = (int)hit.point.x;
                    if (hit.point.x > xMax) xMax = (int)hit.point.x;
                    if (hit.point.z < zMin) zMin = (int)hit.point.z;
                    if (hit.point.z > zMax) zMax = (int)hit.point.z;
                }
            }
        }
        mapInfo.MinX = xMin;
        mapInfo.MaxX = xMax;
        mapInfo.MinY = zMin;
        mapInfo.MaxY = zMax;
        mapInfo.cellList.Clear();
        for (int z = zMin; z < zMax + resolution; z += resolution)
        {
            for (int x = xMin; x < xMax + resolution; x += resolution)
            {
                MapCell cell = new MapCell {x = x, y = z};
                        
                if (Physics.Raycast(new Vector3(x, start.y, z),
                    Vector3.down, 
                    out RaycastHit hit,
                    Math.Abs(start.y - end.y),
                    groundMask | obstacleMask))
                { 
                    cell.h = hit.point.y;
                    if ((((1 << hit.transform.gameObject.layer) & obstacleMask)) != 0)
                    {
                        cell.collision = true;
                    }
                    else if ((((1 << hit.transform.gameObject.layer) & groundMask)) != 0)
                    {
                        cell.collision = false;
                    }
                }
                else
                {
                    cell.collision = true;
                }
                mapInfo.cellList.Add(cell);
            }
        }

        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(mapInfo);
        string filename = $"{Application.dataPath}/{pathPrefix}/{outputName}.txt";
        File.WriteAllText(filename, jsonString);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    
#if UNITY_EDITOR
    private Color validCellColor = new Color(0, 1f, 0, 0.5f);
    private Color collisionCellColor = new Color(1f, 0, 0, 0.5f);
    private void OnDrawGizmosSelected()
    {
        if (showExtracted)
        {
            foreach (var cell in mapInfo.cellList)
            {
                Color cellColor = cell.collision ? collisionCellColor : validCellColor;
                Gizmos.color = cellColor;
                Gizmos.DrawCube(new Vector3(cell.x, cell.h, cell.y), new Vector3(1, 0.3f,1));
            }
        }
    }
#endif
}
