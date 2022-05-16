using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Kame
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class MapInfo
    {
        [JsonProperty] public int MinX { get; set; }
        [JsonProperty] public int MaxX { get; set; }
        [JsonProperty] public int MinY { get; set; }
        [JsonProperty] public int MaxY { get; set; }
        public int SizeX => MaxX - MinX + 1;
        public int SizeY => MaxY - MinY + 1;

        [HideInInspector] [SerializeField]
        public List<MapCell> cellList = new List<MapCell>();
        public Transform[] playerSpawns;
        public Transform[] individualEnemies;
        [JsonProperty] public AreaSpawnInfo[] areaSpawns;
        
        [JsonProperty] private MapCell[,] cells;
        [JsonProperty] private SpawnPoint[] playerSpawnPoints;
        [JsonProperty] private SpawnPoint[] enemySpawnPoints;

        [OnSerializing]
        public void OnBeforeSerialize(StreamingContext context)
        {
            cells = new MapCell[SizeY, SizeX];
            foreach (var cell in cellList)
            {
                cells[(int) cell.y - MinY, (int) cell.x - MinX] = cell;
            }
            
            playerSpawnPoints = new SpawnPoint[playerSpawns.Length];
            for (int i = 0; i < playerSpawns.Length; i++)
            {
                playerSpawnPoints[i] =
                    new SpawnPoint
                    {
                        x = playerSpawns[i].position.x,
                        y = playerSpawns[i].position.z,
                        h = playerSpawns[i].position.y,
                        r = playerSpawns[i].rotation.y
                    };
            }

            enemySpawnPoints = new SpawnPoint[individualEnemies.Length];
            for (int i = 0; i < individualEnemies.Length; i++)
            {
                enemySpawnPoints[i] =
                    new SpawnPoint()
                    {
                        x = individualEnemies[i].position.x,
                        y = individualEnemies[i].position.z,
                        h = individualEnemies[i].position.y,
                        r = individualEnemies[i].rotation.y,
                    };
            }
        }
    }

    [Serializable]
    public class MapCell
    {
        public float x;
        public float y;
        public float h;
        public bool collision = false;

        public MapCell()
        {
        }

        public MapCell(float x, float y, float h, bool collision)
        {
            this.x = x;
            this.y = y;
            this.h = h;
            this.collision = collision;
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AreaSpawnInfo
    {
        public Color EditorColor = new Color(1f, 1f, 0);
        public string Label;
        public int spawnId;
        public Vector3 spawnPosition;

        [HideInInspector] [JsonProperty] public SpawnPoint point;
        [JsonProperty] public float SpawnAreaRadius;
        [JsonProperty] public int MaxGenCount;

        [OnSerializing]
        public void OnBeforeSerialize(StreamingContext context)
        {
            point = new SpawnPoint {id = spawnId, x = spawnPosition.x, y = spawnPosition.z, h = spawnPosition.y};
        }

        [OnDeserialized]
        public void OnAfterDeserialize(StreamingContext context)
        {
            spawnPosition = new Vector3(point.x, point.h, point.y);
            spawnId = point.id;
        }
    }

    [Serializable]
    public struct SpawnPoint
    {
        public int id;
        public float x;
        public float y;
        public float h;
        public float r;
    }
}