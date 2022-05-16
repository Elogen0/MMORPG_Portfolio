using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace InflearnServer.Game
{
	//assume CellSize = 1;
	public class MapCell
	{
		public MapCell() { }
		public MapCell(float x, float y, float h, bool collision)
		{
			this.x = x;
			this.y = y;
			this.h = h;
			this.collision = collision;
		}
		public float x;
		public float y;
		public float h;
		public bool collision = false;
		private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

		public bool Intersects(Circle circle)
		{
			float distanceX = Math.Abs(circle.x - x);
			float distanceY = Math.Abs(circle.y - y);
			float halfSize = 0.5f;
			if (distanceX > halfSize + circle.r) return false;
			if (distanceY > halfSize + circle.r) return false;
			if (distanceX <= halfSize) return true;
			if (distanceY <= halfSize) return true;

			float cornerDistance_sq = (distanceX - halfSize) * (distanceX - halfSize) + (distanceY - halfSize) * (distanceY - halfSize);
			return cornerDistance_sq <= circle.r * circle.r;
		}

		public GameObject GetObject(int id)
		{
			if (_objects.TryGetValue(id, out GameObject obj))
				return obj;
			return null;
		}

		public bool RemoveObject(int id)
		{
			return _objects.Remove(id);
		}

		public void AddObject(GameObject obj)
		{
			_objects.TryAdd(obj.Info.ObjectId, obj);
		}

		public IEnumerable<GameObject> GetObjects()
		{
			foreach (var obj in _objects.Values)
			{
				yield return obj;
			}
		}
	}

	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class AreaSpawnInfo
	{
		[JsonProperty] public SpawnPoint point;
		[JsonProperty] public float SpawnAreaRadius;
		[JsonProperty] public int MaxGenCount;
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
