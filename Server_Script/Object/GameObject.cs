using Google.Protobuf.Protocol;
using InflearnServer.Game.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InflearnServer.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public GameObject Owner { get; set; }

        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public StatInfo StatInfo { get; protected set; } = new StatInfo();
        public PositionInfo PosInfo { get; protected set; } = new PositionInfo()
        {
            State = CreatureState.Idle
        };
        public CharacterStat Stat { get; protected set; } = new CharacterStat();

        public float Speed
        {
            get
            {
                return Stat.GetModifiedValue(StatType.MOVE_SPEED);
            }
            set
            {
                Stat.ChangeBaseValue(StatType.MOVE_SPEED, value);
            }
        }
        public bool IsDead { get; protected set; } = false;
        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = StatInfo;
        }
        public void InitStat(StatInfo statInfo)
        {
            if (DataManager.TryGetStat(statInfo.CharacterId, statInfo.Level, out CharacterStat stat))
            {
                Stat.ChangeBaseValue(stat);
                Stat.Hp = statInfo.Hp;
                Stat.Exp = BigInteger.Parse(statInfo.Exp);
                Stat.OnLevelChanged += OnLevelChanged;
                Stat.OnHpChanged += OnHpChanged;
                Stat.RegisterStatModifiedEvent(OnStatChanged);

                Info.StatInfo.MergeFrom(statInfo);
            }
        }

        public StatInfo GetSyncedStatInfo()
        {
            StatInfo.Hp = Stat.Hp;
            StatInfo.Exp = Stat.Exp.ToString();
            StatInfo.Level = Stat.level;
            return StatInfo;
        }
        
        protected void OnLevelChanged(CharacterStat stat)
        {
            Info.StatInfo.Level = stat.level;
            Info.StatInfo.Hp = stat.Hp;
            Info.StatInfo.Exp = StatInfo.Exp.ToString();
        }

        protected void OnStatChanged(StatValue stat)
        {

        }

        protected void OnHpChanged(float hp)
        {
            StatInfo.Hp = hp;
        }

        public Vector3 Position
        {
            get => new Vector3(Info.PosInfo.PosX, Info.PosInfo.PosY, Info.PosInfo.PosH);
            set
            {
                Info.PosInfo.PosX = value.X;
                Info.PosInfo.PosY = value.Y;
                Info.PosInfo.PosH = value.Z;
            }
        }

        public float Angle(float x, float y)
        {
            float angle = MathF.Atan2(y - PosInfo.PosY, x - PosInfo.PosX) - MathF.Atan2(PosInfo.DirY, PosInfo.DirX);
            if (angle > MathF.PI) 
                angle -= 2 * MathF.PI;
            else if (angle <= -MathF.PI) 
                angle += 2 * MathF.PI;
            //if (angle > 180)
            //    angle = angle - 180;
            angle = MathF.Abs(angle * 180 / MathF.PI);
            return angle;
        }

        public float SqrDistance(Vector3 target)
        {
            return (MathF.Pow(PosInfo.PosX - target.X, 2) + MathF.Pow(PosInfo.PosY - target.Y, 2));
        }

        public void LookAt(Vector3 target)
        {
            Vector3 dist = target - Position;
            float magitude = MathF.Sqrt(dist.X * dist.X + dist.Y * dist.Y);
            PosInfo.DirX = dist.X / magitude;
            PosInfo.DirY = dist.Y / magitude;
        }

        public virtual void OnDamaged(GameObject attacker, float damage)
        {
            if (Room == null)
                return;
            if (IsDead)
                return;
            //todo: 나중에 def 스탯에 영향을 받도록
            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            changePacket.Damage = damage;
            changePacket.AttackerId = attacker.Id;

            Stat.Hp -= damage;
            StatInfo.Hp = Stat.Hp;

            Room.Broadcast(Position, changePacket);

            if (Stat.Hp <= 0)
            {
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;
            IsDead = true;
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(Position, diePacket);

            GameRoom room = Room;
            room.PushAfter(2000, room.LeaveGame, Id);
            //Stat.Hp = Stat.MaxHp;
            //PosInfo.State = CreatureState.Idle;
            //PosInfo.DirX = 0;
            //PosInfo.DirY = -1;
            //PosInfo.PosX = 0;
            //PosInfo.PosY = 0;

            //room.EnterGame(this, randomPos: true);
        }

        public virtual void Update()
        {

        }

        public virtual GameObject GetOwner()
        {
            return this;
        }
    }
}
