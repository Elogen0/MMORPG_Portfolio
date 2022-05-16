using Google.Protobuf.Protocol;
using InflearnServer.DB;
using InflearnServer.Game.Data;
using InflearnServer.Game.Job;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace InflearnServer.Game
{
    public class Monster : GameObject
    {
        public int TempalteId { get; private set; }
        protected CreatureState nextState = CreatureState.Idle;
        
        protected float _skillRange = 3;
        IJob _job;

        Vector3 destination;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public void Init(int templateId)
        {
            TempalteId = templateId;
            if (!DataManager.MonsterDict.TryGetValue(templateId, out EntityData data))
            {
                Console.WriteLine("Cannnot found MonsterId");
                return;
            }    
            Stat = new CharacterStat(data.stat);
            Stat.Hp = data.stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            destination = Position;

            StatInfo.CharacterId = data.stat.id;
            StatInfo.Level = data.stat.level;
            StatInfo.Hp = data.stat.MaxHp;
            StatInfo.Exp = data.stat.TotalExp.ToString();
        }

        public void ChangeState(CreatureState state)
        {
            nextState = state;
        }

        const int updateInterval = 200;
        public override void Update()
        {
            if (IsDead)
                return;
            switch (PosInfo.State)
            {
                case CreatureState.Idle:
                    UpdateIdleState();
                    break;
                case CreatureState.Move:
                    UpdateMoveState();
                    break;
                case CreatureState.Skill:
                    UpdateSkillState();
                    break;
                case CreatureState.Dead:
                    break;
            }


            if (PosInfo.State != nextState)
            {
                OnExitState();
                //Debug.Log($"StateChanged {PosInfo.State} => {nextState}");
                PosInfo.State = nextState;
                OnEnterState();
            }
            if (Room != null)
               _job = Room.PushAfter(updateInterval, Update);
        }

        Player _target; // 방법2 : 타겟의 Id를 들고있는 방법
        float _searchDistance = 10;
        float _chaseDistance = 20;
        int _chaseCellDist = 20; //길찾기 Cell Dist
        long _nextSearchTick = 0;
        protected virtual void UpdateIdleState()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindNearestPlayer(Position, _searchDistance);
            if (target == null)
                return;
            _target = target;
            ChangeState(CreatureState.Move);
            
        }

        long _nextMoveTick = 0;

        protected virtual void UpdateMoveState()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                ChangeState(CreatureState.Idle);
                BroadcastMove();
                return;
            }

            float targetSqrDistance = _target.SqrDistance(Position);
            if (targetSqrDistance > _chaseDistance * _chaseDistance)
            {
                _target = null;
                ChangeState(CreatureState.Idle);
                BroadcastMove();
                return;
            }

            if (targetSqrDistance <= _skillRange * _skillRange)
            {
                _coolTick = 0;
                ChangeState(CreatureState.Skill);
                BroadcastMove();
                return;
            }


            //if (SqrDistance(destination) < 0.1)
            //{
                List<Vector3> path = Room.Map.FindPath(Position, _target.Position, false);
                if (path.Count < 2 || path.Count > _chaseCellDist) //0번째 인덱스는 자신의 위치이므로 2보다 작으면 갈수있는 길이 없다.
                {
                    _target = null;
                    ChangeState(CreatureState.Idle);
                    BroadcastMove();
                    return;
                }
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            destination.X = path[1].X + rand.Next(-3, 4) / 10;
            destination.Y = path[1].Y + rand.Next(-3, 4) / 10;
                //destination = path[1];

                LookAt(destination);
                if (Room.Map.ApplyMove(this, destination))
                    BroadcastMove();
            //}
            //else
            //{
            //    //LookAt(destination);
            //    //Vector2 newPos = Vector2.Lerp(Position, destination, (updateInterval / 1000));
            //    //if (Room.Map.ApplyMove(this, newPos))
            //    //    BroadcastMove();
            //}
        }

        void BroadcastMove()
        {
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(Position, movePacket);
        }


        long _coolTick = 0;
        protected virtual void UpdateSkillState()
        {
            
            if (_coolTick == 0)
            {

                // 유효한 타깃인지
                if (_target == null || _target.Room != Room || _target.Stat.Hp == 0)
                {
                    _target = null;
                    ChangeState(CreatureState.Move);
                    BroadcastMove();
                    return;
                }
                //스킬이 아직 사용 가능한지
                float sqrDistance = _target.SqrDistance(Position);
                if (sqrDistance > _skillRange * _skillRange)
                {
                    ChangeState(CreatureState.Idle);
                    BroadcastMove();
                    return;
                }

                //타게팅 방향 주시
                LookAt(_target.Position);
                BroadcastMove();

                //데미지 판정
                //todo : 몬스터 데이터시트를 따로빼서 몬스터데이터 시트에서 스킬 데이터를 가져오기
                if (DataManager.SkillDict.TryGetValue(1, out SkillData skillData))
                {
                    _target.OnDamaged(this, skillData.damage + Stat.GetModifiedValue(StatType.ATK));
                    //스킬 사용 Boradcast
                    //Console.WriteLine($"{skillData.id}({Stat.id}) damage : {skillData.damage} + {Stat.GetModifiedValue(StatType.ATK)}");
                    S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
                    {
                        skillPacket.ObjectId = Id;
                        skillPacket.Info.SkillId = skillData.id;
                        Room.Broadcast(Position, skillPacket);
                    }
                }
                //스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }
            if (_coolTick > Environment.TickCount64)
                return;
            _coolTick = 0;
        }

        protected virtual void OnExitState()
        {
            switch (PosInfo.State)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Move:
                    break;
                case CreatureState.Skill:
                    break;
                case CreatureState.Dead:
                    break;
            }
        }

        protected virtual void OnEnterState()
        {
            switch (PosInfo.State)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Move:
                    break;
                case CreatureState.Skill:
                    break;
                case CreatureState.Dead:
                    break;
            }
        }

        public override void OnDamaged(GameObject attacker, float damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            if (_job != null)
            {
                _job.Cancel = true;
                _job = null;
            }

            base.OnDead(attacker);
            Player player = (Player)attacker.Owner;
            player.Quest.ProcessQuest(QuestType.DestroyEnemy, TempalteId, 1);
            if (attacker.GetOwner().ObjectType == GameObjectType.Player)
            {
                RewardData rewardData = GetRamdomReward();
                if (rewardData != null)
                {
                    DBTransaction.RewardPlayer(player, rewardData, Room);
                }
            }
        }

        RewardData GetRamdomReward()
        {
            if (!DataManager.MonsterDict.TryGetValue(TempalteId, out EntityData monsterData))
                return null;

            int rand = new Random().Next(0, 101);

            // rand = 0~100 -> 42
            // 10 10 10 10 10
            int sum = 0;
            foreach (RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;
                if (rand <= sum)
                {
                    return rewardData;
                }
            }

            return null;
        }

    }
}
