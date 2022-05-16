using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InflearnServer.Game
{
    class Arrow : Projectile
    {
        public override void Update()
        {
            if (SkillData == null || Owner == null || Room == null)
                return;

            int tick = (int)(1000 / Speed);
            Room.PushAfter(tick, Update);

            Vector3 nextPos =
                new Vector3(PosInfo.PosX + PosInfo.DirX * Speed * tick / 1000, PosInfo.PosY + PosInfo.DirY * Speed * tick / 1000, PosInfo.PosH);

            if (Room.Map.ApplyMove(this, nextPos))
            {
                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(nextPos, movePacket);
            }
            else
            {
                //장애물 소멸
                Room.Push(Room.LeaveGame,Id);
                return;
            }

            //todo :피격 판정 & 소멸
            float nearestDistance = float.MaxValue;
            GameObject nearestObject = null;

            foreach (var obj in Room.Map.GetObjects(nextPos))
            {
                MapCell cell = Room.Map.GetCell(nextPos);
                List<MapCell> cells = Room.Map.GetCells(nextPos);
                if (obj == this.Owner)
                    continue;
                if (obj == this)
                    continue;
                float sqrdistance = Owner.SqrDistance(new Vector3(obj.PosInfo.PosX, obj.PosInfo.PosY, obj.PosInfo.PosH));
                if (sqrdistance < nearestDistance)
                {
                    nearestDistance = sqrdistance;
                    nearestObject = obj;
                }
            }
            if (nearestObject != null)
            {
                Console.WriteLine($"Hit Arrow : {nearestObject.Id}");
                nearestObject.OnDamaged(this, SkillData.damage + Owner.Stat.GetModifiedValue(Data.StatType.ATK));
                Room.Push(Room.LeaveGame, Id);
            }
            //todo : arrow가 Player 방향을 따라감... float로 바꾸자!
            //PositionInfo, PosX,PosYDirX,DirY
        }

        public override GameObject GetOwner()
        {
            return Owner;
        }

        public override void OnDamaged(GameObject attacker, float damage)
        {
            
        }

        public override void OnDead(GameObject attacker)
        {
            
        }
    }
}
