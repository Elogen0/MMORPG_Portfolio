using Google.Protobuf.Protocol;
using Newtonsoft.Json;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace InflearnServer.Game
{
    public struct ArrPos
	{
		public ArrPos(int y, int x) { X = x; Y = y; }
		public int X;
		public int Y;

		public static bool operator ==(ArrPos lhs, ArrPos rhs)
        {
			return lhs.Y == rhs.Y && lhs.X == rhs.X;
        }

		public static bool operator !=(ArrPos lhs, ArrPos rhs)
        {
			return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
			return (ArrPos)obj == this;
        }

        public override int GetHashCode()
        {
			long value = (Y << 32) | X;
			return value.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

	public struct PQNode : IComparable<PQNode>
	{
		public int F;
		public int G;
		public int Y;
		public int X;

		public int CompareTo(PQNode other)
		{
			if (F == other.F)
				return 0;
			return F < other.F ? 1 : -1;
		}
	}

	public struct Vector2Int
	{
		public int x;
		public int y;

		public Vector2Int(int x, int y) { this.x = x; this.y = y; }

		public Vector2Int(Vector2 other)
        {
			x = (int)other.X;
			y = (int)other.Y;
        }

		public static Vector2Int up { get { return new Vector2Int(0, 1); } }
		public static Vector2Int down { get { return new Vector2Int(0, -1); } }
		public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
		public static Vector2Int right { get { return new Vector2Int(1, 0); } }

		public static Vector2Int operator +(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.x + b.x, a.y + b.y);
		}

		public static Vector2Int operator -(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.x - b.x, a.y - b.y);
		}

		public float magnitude { get { return (float)Math.Sqrt(sqrMagnitude); } }
		public int sqrMagnitude { get { return (x * x + y * y); } }
		public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }
	}


	public struct Circle
	{
		public Circle(Vector2 position, float size) 
		{ this.x = position.X; this.y = position.Y; this.size = size; }
		public float x;
		public float y;
		public float size;
		public float r => size / 2;
	}

	

	public class Map
	{
		public int MinX { get; set; }
		public int MaxX { get; set; }
		public int MinY { get; set; }
		public int MaxY { get; set; }

		public int SizeX { get { return MaxX - MinX + 1; } }
		public int SizeY { get { return MaxY - MinY + 1; } }

		MapCell[,] cells;
		public AreaSpawnInfo[] areaSpawns;
		public SpawnPoint[] playerSpawnPoints;
		public SpawnPoint[] enemySpawnPoints;

		public MapCell GetCell(Vector3 position)
		{
			if (position.X < MinX || position.X > MaxX)
				return null;
			if (position.Y < MinY || position.Y > MaxY)
				return null;

			return cells[(int)position.Y - MinY, (int)position.X - MinX];
		}

		public List<MapCell> GetCells(Vector3 position, float findRadius = 0f)
		{
			List<MapCell> cells = new List<MapCell>();
			float cellMinX = position.X - MathF.Round(findRadius + 0.001f);
			float cellMaxX = position.X + MathF.Round(findRadius + 0.001f);
			float cellMinY = position.Y - MathF.Round(findRadius + 0.001f);
			float cellMaxY = position.Y + MathF.Round(findRadius + 0.001f);
			for (float y = cellMinY; y <= cellMaxY; y++)
			{
				for (float x = cellMinX; x <= cellMaxX; x++)
				{
					MapCell cell = GetCell(new Vector3(x, y, 0));
					if (cell == null)
						continue;
					cells.Add(cell);
				}
			}
			return cells;
		}

		public List<GameObject> GetObjects(Vector3 position, float findRadius = 0f)
		{
			List<GameObject> objList = new List<GameObject>();
			if (findRadius == 0)
            {
				var cell = GetCell(position);
				if (cell == null)
					return objList;
                foreach (var obj in cell.GetObjects())
                {
					objList.Add(obj);
                }
			}
			else
            {
				var cells = GetCells(position, findRadius);
				foreach (var cell in cells)
				{
					foreach (var obj in cell.GetObjects())
					{
                        if (obj.SqrDistance(position) <= findRadius * findRadius)
                        {
							objList.Add(obj);
                        }
                    }
				}
			}
			return objList;
		}

		public bool AddObject(GameObject go)
        {
			if (go == null)
				return false;
			GetCell(go.Position).AddObject(go);
			return true;
        }

		public bool CanGo(Vector3 position, bool searchCell = true)
		{
			MapCell cell = GetCell(position);
			if (cell == null)
            {
				return false;
			}
			if (searchCell && cell.collision)
			{
				return false;
			}

			//if (cell.collision == true)
   //         {
			//	if (cell.Intersects(new Circle(position, 1)))
			//	{
			//		return false;
			//	}
			//}
			return true;
		}

		
		public bool ApplyLeave(GameObject gameObject)
        {
			if (gameObject.Room == null)
				return false;
			if (gameObject.Room.Map != this)
				return false;

			MapCell cell = GetCell(gameObject.Position);
			if (cell == null)
				return false;

			gameObject.Room.GetZone(gameObject.Position)
				.Remove(gameObject);

			return cell.RemoveObject(gameObject.Info.ObjectId);
        }

		public bool ApplyMove(GameObject gameObject, Vector3 destination, bool onGround = true)
        {
			if (gameObject.Room == null)
				return false;
			if (gameObject.Room.Map != this)
				return false;

			PositionInfo posInfo = gameObject.PosInfo;

			bool canGo = CanGo(destination);
			if ( canGo == false)
				return false;

			int curX  = (int)(posInfo.PosX);
			int curY  = (int)(posInfo.PosY);
			int nextX = (int)(destination.X);
			int nextY = (int)(destination.Y);

			//cell위치가 바뀌었으면
			float mapHeight = 0;
			if (curX != nextX || curY != nextY)
            {
                //기존 위치에서 삭제
                GetCell(new Vector3(curX, curY, 0))?.RemoveObject(gameObject.Info.ObjectId);
				//새로운 위치에 추가
				MapCell nextCell = GetCell(new Vector3(nextX, nextY, 0));
				nextCell.AddObject(gameObject);
				mapHeight = nextCell.h;
			}
			//실제 좌표이동
			posInfo.PosX = destination.X;
			posInfo.PosY = destination.Y;
			posInfo.PosH = onGround ? mapHeight: destination.Z;
			//{
			//    Console.WriteLine($"[Move] Id:{gameObject.Id} ({nextX},{nextY},{GetCell(nextX, nextY)._objects.Count})");
			//}

			//zone
			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
			Zone nowZone = gameObject.Room.GetZone(new Vector3(gameObject.PosInfo.PosX, gameObject.PosInfo.PosY, gameObject.PosInfo.PosH));
			Zone afterZone = gameObject.Room.GetZone(destination); //? next
			if (type == GameObjectType.Player)
            {
				Player player = (Player)gameObject;
				if (nowZone != afterZone)
				{
					if (nowZone != afterZone)
					{
						nowZone.Players.Remove(player);
                        afterZone.Players.Add(player);
					}
				}
			}
			else if(type == GameObjectType.Monster)
            {
				Monster monster = (Monster)gameObject;

				if (nowZone != afterZone)
				{
					if (nowZone != afterZone)
					{
						nowZone.Monsters.Remove(monster);
						afterZone.Monsters.Add(monster);
					}
				}
			}
			else if (type == GameObjectType.Projectile)
            {
				Projectile projectile = (Projectile)gameObject;

				if (nowZone != afterZone)
				{
					if (nowZone != afterZone)
					{
						nowZone.Projectiles.Remove(projectile);
						afterZone.Projectiles.Add(projectile);
					}
				}
			}

            
			return true;
        }

		//todo : 나중에 데이터매니저에서 경로까지
		public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
		{
			//todo : 데이터메니저에서 어떤지역에선 어떤맵을 로드하세요
			string mapName = "Map_" + mapId.ToString("000");

			// Collision 관련 파일
			string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");

			MapInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<MapInfo>(text);

			MinX = info.MinX;
			MaxX = info.MaxX;
			MinY = info.MinY;
			MaxY = info.MaxY;

			cells = info.cells;
			areaSpawns = info.areaSpawns;
			playerSpawnPoints = info.playerSpawnPoints;
			enemySpawnPoints = info.enemySpawnPoints;
		}

		#region A* PathFinding

		// U D L R UL DL DR UR
		int[] _deltaY = new int[] { 1, -1, 0, 0, 1, -1, -1, 1 };
		int[] _deltaX = new int[] { 0, 0, -1, 1, -1, -1, 1, 1 };
		int[] _cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startCellPos"></param>
		/// <param name="destCellPos"></param>
		/// <param name="checkObject"> true : 길찾기할때 다른 오브젝트들도 처리, false : 벽만 처리</param> //todo : 콜리전을 서클로처리
		/// <returns></returns>
		public List<Vector3> FindPath(Vector3 startCellPos, Vector3 destCellPos, bool checkObject = false, int maxDist = 10)
		{
			List<ArrPos> path = new List<ArrPos>();

			// 점수 매기기
			// F = G + H
			// F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
			// G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
			// H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

			// (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
			HashSet<ArrPos> closeList = new HashSet<ArrPos>();

			// (y, x) 가는 길을 한 번이라도 발견했는지
			// 발견X => MaxValue
			// 발견O => F = G + H
			Dictionary<ArrPos, int> openList = new Dictionary<ArrPos, int>(); //open List
			Dictionary<ArrPos, ArrPos> parent = new Dictionary<ArrPos, ArrPos>();

			// 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
			PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

			// CellPos -> ArrayPos
			ArrPos pos = World2ArrPosition(startCellPos);
			ArrPos dest = World2ArrPosition(destCellPos);

			// 시작점 발견 (예약 진행)
			openList.Add(pos, 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));
			pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
			parent.Add(pos, pos);

			while (pq.Count > 0)
			{
				// 제일 좋은 후보를 찾는다
				PQNode pqNode = pq.Pop();
				ArrPos node = new ArrPos(pqNode.Y, pqNode.X);
				// 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
				if (closeList.Contains(node))
					continue;

				// 방문한다
				closeList.Add(node);
				// 목적지 도착했으면 바로 종료
				if (pqNode.Y == dest.Y && pqNode.X == dest.X)
					break;

				// 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
				for (int i = 0; i < _deltaY.Length; i++)
				{
					ArrPos next = new ArrPos(pqNode.Y + _deltaY[i], pqNode.X + _deltaX[i]);

					if (Math.Abs(pos.Y - next.Y) + Math.Abs(pos.X - next.X) >= maxDist)
						continue;

					// 유효 범위를 벗어났으면 스킵
					if (next.X < 0 || next.X >= SizeX || next.Y < 0 || next.Y >= SizeY )
						continue;
					// 벽으로 막혀서 갈 수 없으면 스킵
					if (next.Y != dest.Y || next.X != dest.X)
					{
						Vector3 cellPosition = Arr2WorldPosition(next);
						bool canGo = CanGo(cellPosition, true);
						if ( canGo == false) // CellPos //todo : 서클로처리
							continue;
					}

					// 이미 방문한 곳이면 스킵
					if (closeList.Contains(next))
						continue;

					// 비용 계산
					int g = 0;// node.G + _cost[i];
					int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
					// 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
					int value = 0;
					if (openList.TryGetValue(next, out value) == false)
                    {
						value = Int32.MaxValue;
                    }
					if (value < g + h)
						continue;

					// 예약 진행
					if (openList.TryAdd(next, g+h) == false) //false : 이미값이 들어가있음
						openList[next] = g + h;

					pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
					if (parent.TryAdd(next, node) == false)
						parent[next] = node;
				}
			}

			return CalcCellPathFromParent(parent, dest);
		}

		List<Vector3> CalcCellPathFromParent(Dictionary<ArrPos, ArrPos> parent, ArrPos dest)
		{
			List<Vector3> cells = new List<Vector3>();

			//todo: Astar 알고리즘 서칭할때 하도록... 지금은 2중 서치
			if (parent.ContainsKey(dest) == false) //길이 없으면
            {
				ArrPos alterDest = new ArrPos();
				int bestDist = Int32.MaxValue;

                foreach (ArrPos pos in parent.Keys)
                {
					int dist = Math.Abs(dest.X - pos.X) + Math.Abs(dest.Y - pos.Y);
                    if (dist < bestDist)
					{
						alterDest = pos;
						bestDist = dist;
                    }
                }
				dest = alterDest;
            }

            {
				ArrPos pos = dest;
				while (parent[pos] != pos)
				{
					cells.Add(Arr2WorldPosition(pos));
					pos = parent[pos];
				}
				cells.Add(Arr2WorldPosition(pos));
				cells.Reverse();
			}
			

			return cells;
		}

		public ArrPos World2ArrPosition(Vector3 cellPos)
		{
			// CellPos -> ArrayPos
			return new ArrPos((int)cellPos.Y - MinY, (int)cellPos.X - MinX);
		}

		public Vector3 Arr2WorldPosition(ArrPos pos)
		{
			// ArrayPos -> CellPos
			MapCell cell = cells[pos.Y, pos.X];
			return new Vector3(cell.x, cell.y, cell.h);
		}

		#endregion
	}
}
