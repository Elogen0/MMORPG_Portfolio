using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
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

        [JsonProperty] public MapCell[,] cells;
        [JsonProperty] public AreaSpawnInfo[] areaSpawns;
        [JsonProperty] public SpawnPoint[] playerSpawnPoints;
        [JsonProperty] public SpawnPoint[] enemySpawnPoints;
    }
}
