using Google.Protobuf;
using Google.Protobuf.Protocol;
using InflearnServer.Game.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace InflearnServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public const float VisionRadius = 20;
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        Dictionary<int, int> _monsterCount = new Dictionary<int, int>();

        public Zone[,] Zones { get; private set; }
        public int ZoneCells { get; private set; }

        public Map Map { get; private set; } = new Map();

        public static int GetRoomIdBySceneName(string sceneName)
        {
            //todo : sceneName으로 MapID찾기
            int roomId = -1;
            if (sceneName == "town")
            {
                roomId = 3;
            }
            else if (sceneName == "dungeons")
            {
                roomId = 2;
            }
            return roomId;
        }

        public Zone GetZone(Vector3 position)
        {
            ArrPos arrPos = Map.World2ArrPosition(position);
            int zoneX = arrPos.X / ZoneCells;
            int zoneY = arrPos.Y / ZoneCells;
            //int zoneX = ((int)position.X - Map.MinX) / ZoneCells;
            //int zoneY = ((int)position.Y - Map.MinY) / ZoneCells;

            if (zoneX < 0 || zoneX >= Zones.GetLength(1))
                return null;
            if (zoneY < 0 || zoneY >= Zones.GetLength(0))
                return null;
            return Zones[zoneY, zoneX];
        }
        
        
        public void Init(int mapId, int zoneCells)
        {
            Map.LoadMap(mapId);

            //Zone
            ZoneCells = zoneCells;
            int countY = (Map.SizeY + zoneCells -1) / zoneCells;
            int countX = (Map.SizeX + zoneCells -1) / zoneCells;
            Zones = new Zone[countY, countX];
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    Zones[y, x] = new Zone(x, y);
                }
            }

            
            foreach (var spawnInfo in Map.areaSpawns)
            {
                _monsterCount.Add(spawnInfo.point.id, spawnInfo.MaxGenCount);
                for (int i = 0; i < spawnInfo.MaxGenCount; i++)
                {
                    Monster monster = ObjectManager.Instance.Add<Monster>();
                    int spawnPointX;
                    int spawnPointY;
                    float mapHeight;
                    Vector3 tempPos;
                    do
                    {
                        spawnPointX = _rand.Next((int)(spawnInfo.point.x - spawnInfo.SpawnAreaRadius), (int)(spawnInfo.point.x + spawnInfo.SpawnAreaRadius));
                        spawnPointY = _rand.Next((int)(spawnInfo.point.y - spawnInfo.SpawnAreaRadius), (int)(spawnInfo.point.y + spawnInfo.SpawnAreaRadius));
                        tempPos = new Vector3(spawnPointX, spawnPointY, 0);
                        mapHeight = Map.GetCell(tempPos).h;
                    } while (!Map.CanGo(tempPos));

                    monster.Position = new Vector3(spawnPointX, spawnPointY, mapHeight);
                    monster.Init(spawnInfo.point.id);

                    EnterGame(monster, randomPos: false);
                }
                PushAfter<AreaSpawnInfo, int>(2000, RespawnMonster, spawnInfo, 2000);
            }
        }

        //누군가가 주기적으로 호출해 줘야한다.
        public void Update()
        {
            Flush();
        }

        Random _rand = new Random();

        public void EnterGame(GameObject gameObject, bool randomPos)
        {
            if (gameObject == null)
                return;

            if (randomPos)
            {
                Vector3 respawnPos;
                while (true)
                {
                    respawnPos.X = _rand.Next(Map.MinX, Map.MaxX + 1);
                    respawnPos.Y = _rand.Next(Map.MinY, Map.MaxY + 1);
                    respawnPos.Z = 0;
                    respawnPos.Z = Map.GetCell(respawnPos).h;
                    if (Map.CanGo(respawnPos))
                    {
                        gameObject.Position = respawnPos;
                        break;
                    }
                }
            }

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player newPlayer = gameObject as Player;
                //newPlayer.PosInfo.PosX = 1;
                //newPlayer.PosInfo.PosY = 1;
                _players.Add(newPlayer.Id, newPlayer);
                newPlayer.Room = this;
                newPlayer.Owner = newPlayer;
                
                Map.AddObject(newPlayer);//ApplyMove?
                GetZone(newPlayer.Position).Players.Add(newPlayer);

                //본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    //todo: 저장된 마지막 위치 전송
                    newPlayer.Session.Send(enterPacket);

                    newPlayer.Vision.Update();
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                monster.Info.Name = "Monseter_" + monster.Id;

                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;
                Map.AddObject(monster);//ApplyMove?
                
                GetZone(monster.Position).Monsters.Add(monster);

                monster.Update();
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                projectile.Info.Name = "Projectile_" + projectile.Id;
                _projectiles.Add(gameObject.Id, projectile);

                Map.AddObject(projectile);//ApplyMove?
                GetZone(projectile.Position).Projectiles.Add(projectile);
                projectile.Room = this;

                projectile.Update();
            }
               
            //타인한테 정보전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                Broadcast(gameObject.Position, spawnPacket);
            }
        }

        public void LeaveGame(int objectId)
        {
            Vector3 position;
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);
            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;
                position = player.Position;

                player.OnLeaveGame();
                Map.ApplyLeave(player);
                player.Room = null;

                //본인한테 정보전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;

                if (_monsters.Remove(objectId, out monster) == false)
                    return;
                if (_monsterCount.ContainsKey(monster.StatInfo.CharacterId))
                {
                    --_monsterCount[monster.StatInfo.CharacterId];
                }

                position = monster.Position;
                Map.ApplyLeave(monster);
                monster.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;
                position = projectile.Position;
                Map.ApplyLeave(projectile);

                projectile.Room = null;
            }
            else
            {
                return;
            }

            //타인한테 정보전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                Broadcast(position, despawnPacket);
            }
        }

        //todo : 범위안에 플레이어만 찾게 성능개선
        public Player FindNearestPlayer(Vector3 position, float range)
        {
            Player nearestPlayer = null;
            float nearestDistance = float.MaxValue;
            List<GameObject> gos = Map.GetObjects(position, range);
            foreach (var obj in gos)
            {
                var type = ObjectManager.GetObjectTypeById(obj.Id);
                if (type == GameObjectType.Player)
                {
                    float distance = obj.SqrDistance(position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = (Player)obj;
                    }
                }
            }
            return nearestPlayer;
        }

        public void Broadcast(Vector3 position, IMessage packet)
        {
            foreach (GameObject obj in Map.GetObjects(position, VisionRadius))
            {
                Player player = obj as Player;
                if (player != null)
                    player.Session.Send(packet);
            }
        }

        public List<Player> GetAdjacentPlayers(Vector3 position, float range)
        {
            List <Zone> zones = GetAdjacentZones(position, range);
            return zones.SelectMany(z => z.Players).ToList();
        }

        //DummyClient #3 적용 안된버전
        public List<Zone> GetAdjacentZones(Vector3 position, float range = GameRoom.VisionRadius)
        {
            HashSet<Zone> zones = new HashSet<Zone>();

            float[] delta = new float[2] { -range, +range };
            foreach (float dy in delta)
            {
                foreach (float dx in delta)
                {
                    float searchX = position.X + dx;
                    float searchY = position.Y + dy;
                    Zone zone = GetZone(new Vector3(searchX, searchY, 0));
                    if (zone == null)
                        continue;

                    zones.Add(zone);
                }
            }

            return zones.ToList();
        }

        public void RespawnMonster(AreaSpawnInfo spawnInfo, int respawnTime)
        {
            if (!_monsterCount.ContainsKey(spawnInfo.point.id))
            {
                Console.WriteLine($"{spawnInfo.point.id} not contains key");
                return;
            }

            if (_monsterCount[spawnInfo.point.id] < spawnInfo.MaxGenCount)
            {
                Console.WriteLine($"RespawnMonster... {spawnInfo.point.id}:{_monsterCount[spawnInfo.point.id]}");

                Monster monster = ObjectManager.Instance.Add<Monster>();

                int spawnPointX;
                int spawnPointY;
                float mapHeight;
                Vector3 tempPos;
                do
                {
                    spawnPointX = _rand.Next((int)(spawnInfo.point.x - spawnInfo.SpawnAreaRadius), (int)(spawnInfo.point.x + spawnInfo.SpawnAreaRadius));
                    spawnPointY = _rand.Next((int)(spawnInfo.point.y - spawnInfo.SpawnAreaRadius), (int)(spawnInfo.point.y + spawnInfo.SpawnAreaRadius));
                    tempPos = new Vector3(spawnPointX, spawnPointY, 0);
                    mapHeight = Map.GetCell(tempPos).h;
                } while (!Map.CanGo(tempPos));

                monster.Position = new Vector3(spawnPointX, spawnPointY, mapHeight);
                monster.Init(spawnInfo.point.id);
                ++_monsterCount[spawnInfo.point.id];

                EnterGame(monster, randomPos: false);
            }
            PushAfter<AreaSpawnInfo, int>(respawnTime, RespawnMonster, spawnInfo, respawnTime);
        }

        public void HandleAnimatorParameter(C_AnimParameter parameter, GameObject player)
        {
            S_AnimParameter s_parameter = new S_AnimParameter
            {
                ObjectId = player.Id,
                Type = parameter.Type,
                HashKey = parameter.HashKey,
                Value = parameter.Value
            };

            Broadcast(player.Position, s_parameter);
        }
    }
}
