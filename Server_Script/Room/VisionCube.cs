using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InflearnServer.Game
{
    public class VisionCube
    {
        public Player Owner { get; private set; }
        public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

        public VisionCube(Player owner)
        {
            Owner = owner;
        }

        public HashSet<GameObject> GatherObjects()
        {
            if (Owner == null || Owner.Room == null || Owner.Room.Map == null)
                return null;

            HashSet<GameObject> objects = new HashSet<GameObject>();

            foreach (var obj in Owner.Room.Map.GetObjects(Owner.Position, GameRoom.VisionRadius))
            {
                objects.Add(obj);
            } 

            return objects;
        }

        public void Update()
        {
            if (Owner == null || Owner.Room == null)
                return;

            HashSet<GameObject> currentObjects = GatherObjects();

            //기존엔 없었는데 새로생긴애들 Spawn처리 기존 ToList -> IEnumearble그대로 사용하도록
            List<GameObject> added = currentObjects.Except(PreviousObjects).ToList();
            if (added.Count() > 0)
            {
                S_Spawn spawnPacket = new S_Spawn();
                foreach (var gameObject in added)
                {
                    ObjectInfo info = new ObjectInfo();
                    info.MergeFrom(gameObject.Info);
                    spawnPacket.Objects.Add(info);
                }

                Owner.Session.Send(spawnPacket);
            }

            //기존엔 있었는데 사라진
            List<GameObject> removed = PreviousObjects.Except(currentObjects).ToList();
            if (removed.Count() > 0)
            {
                S_Despawn despawnPacket = new S_Despawn();
                foreach (var gameObject in removed)
                {
                    despawnPacket.ObjectIds.Add(gameObject.Id);
                }

                Owner.Session.Send(despawnPacket);
            }

            PreviousObjects = currentObjects;

            Owner.Room.PushAfter(100, Update);
        }
    }
}
