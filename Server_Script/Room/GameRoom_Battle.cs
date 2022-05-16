using Google.Protobuf;
using Google.Protobuf.Protocol;
using InflearnServer.Game.Data;
using InflearnServer.Game.Job;
using Kame.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace InflearnServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            //Todo : 검증
            //일단 서버에서 좌표이동
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo playerInfo = player.Info;
            //다른 좌표로 이동할 경우, 갈 수 있는지 체크
            //{
            //    if (Map.CanGo(movePosInfo.PosX, movePosInfo.PosY) == false)
            //        return;
            //}
            Map.ApplyMove(player, new Vector3(movePosInfo.PosX, movePosInfo.PosY, movePosInfo.PosH), false);
            playerInfo.PosInfo.State = movePosInfo.State;
            playerInfo.PosInfo.DirX = movePosInfo.DirX;
            playerInfo.PosInfo.DirY = movePosInfo.DirY;

            //다른 플레이어 한테도 알려준다.
            S_Move smPkt = new S_Move();
            smPkt.ObjectId = player.Info.ObjectId;
            smPkt.PosInfo = new PositionInfo();
            smPkt.PosInfo.MergeFrom(movePacket.PosInfo);
            //smPkt.PosInfo = playerInfo.PosInfo;

            Broadcast(player.Position, smPkt);
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;
            SkillInfo skillInfo = skillPacket.Info;
            PositionInfo skillPos = skillInfo.PosInfo;
            ObjectInfo info = player.Info;

            //Console.WriteLine($"skillPos : [{skillPos.PosX},{skillPos.PosY}]");
            if (info.PosInfo.State == CreatureState.Dead)
            {
                return;
            }
            //todo : 스킬 사용 가능 여부 체크
            info.PosInfo.State = CreatureState.Skill;

            S_Skill skill = new S_Skill() { Info = new SkillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillInfo.SkillId;
            Broadcast(player.Position, skill);

            if (DataManager.SkillDict.TryGetValue(skillInfo.SkillId, out SkillData skillData) == false)
                return;
            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:

                    break;

                case SkillType.SkillProjectile:
                    Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = player;
                    arrow.SkillData = skillData;

                    arrow.PosInfo.State = CreatureState.Move;
                    arrow.PosInfo.DirX = player.PosInfo.DirX;
                    arrow.PosInfo.DirY = player.PosInfo.DirY;
                    arrow.PosInfo.PosX = player.PosInfo.PosX;
                    arrow.PosInfo.PosY = player.PosInfo.PosY;
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow, false);
                    break;
                case SkillType.SkillCircle:
                    foreach (var obj in Map.GetObjects(new Vector3(skillPos.PosX, skillPos.PosY, skillPos.PosH), skillData.rangeX))
                    {
                        if (obj == player)
                            continue;
                        if (obj.IsDead)
                            continue;
                        //Console.WriteLine($"Angle : {MathHelper.Angle(new Vector2(skillPos.PosX, skillPos.PosY), new Vector2(obj.PosInfo.PosX, obj.PosInfo.PosY), new Vector2(skillPos.DirX, skillPos.DirY))}");
                        if (skillData.angle != 0)
                        {
                            if (MathHelper.Angle(
                                new Vector2(skillPos.PosX, skillPos.PosY), 
                                new Vector2(obj.PosInfo.PosX, obj.PosInfo.PosY), 
                                new Vector2(skillPos.DirX, skillPos.DirY)) > skillData.angle / 2)
                                continue;
                        }
                        var objType = ObjectManager.GetObjectTypeById(obj.Id);
                        if (objType == GameObjectType.Monster)
                            obj.OnDamaged(player, skillData.damage + player.Stat.GetModifiedValue(StatType.ATK));
                        //player.Stat.GetModifiedValue(StatType.ATK)
                    }
                    break;
                case SkillType.SkillRect:
                    break;
            }
                //통과
        }

        public Vector2 GetRandomInCircle(Vector2 position, float radius)
        {
            Random rand = new Random();
            var angle = rand.NextDouble() * Math.PI * 2;
            var r = Math.Sqrt(rand.NextDouble()) * radius;
            var x = position.X + r * Math.Cos(angle);
            var y = position.Y + r * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }
    }
}
