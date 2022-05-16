using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game.Data
{
    #region Skill
    [System.Serializable]
    public class SkillData : IManagedData
    {
        public int ID => id;

        public int id;
        public string name;
        public string description;
        public string tag;
        public float cooldown;
        public float attackDelay;
        public float damage;
        public float rangeX;
        public float rangeY;
        public float angle;
        public SkillType skillType;
        public ProjectileInfo projectile;

        public SkillData()
        { }

        public SkillData(SkillData other)
        {
            id = other.id;
            name = other.name;
            description = other.description;
            tag = other.tag;
            cooldown = other.cooldown;
            damage = other.damage;
            rangeX = other.rangeX;
            rangeY = other.rangeY;
            angle = other.angle;
            skillType = other.skillType;
            projectile = other.projectile;
        }
    }

    [System.Serializable]
    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;

        public ProjectileInfo()
        {
        }

        public ProjectileInfo(ProjectileInfo other)
        {
            name = other.name;
            speed = other.speed;
            range = other.range;
            prefab = other.prefab;
        }
    }
    #endregion

    #region Monster
    [System.Serializable]
    public class RewardData // todo : 클라에선 삭제
    {
        public int probability; //100분율
        public int itemId;
        public int amount;

        public RewardData() { }

        public RewardData(RewardData other)
        {
            this.probability = other.probability;
            this.itemId = other.itemId;
            this.amount = other.amount;
        }
    }

    public enum GameEntityType
    {
        None = -1,
        Player = 0,
        OtherPlayer = 1,
        Friend = 2,
        #region Objects
        Object = 10000000,
        #endregion
        #region NPC
        NPC = 20000000,
        #endregion
        #region Monster
        Monster = 30000000,
        #endregion
    }

    public class EntityData : IManagedData
    {
        public GameEntityType type;
        public string name;
        public bool attackable = false;
        public CharacterStat stat = new CharacterStat();
        public List<RewardData> rewards = new List<RewardData>();
        public string objectPath;

        public int ID => stat.id;
    }

    #endregion

    #region Dialogue and Quest
    [Serializable]
    public class QuestData : IManagedData
    {
        public int ID => id;
        public int id;
        public string reference;
        public string title;
        public string description;
        public QuestObjective objective = new QuestObjective();
        public List<RewardData> rewards = new List<RewardData>();

        public QuestData() { }
        public QuestData(QuestData other)
        {
            this.id = other.id;
            this.reference = other.reference;
            this.title = other.title;
            this.description = other.description;
            this.objective = other.objective;
            this.rewards = other.rewards.ConvertAll(r => new RewardData(r));
        }
    }

    public enum QuestType
    {
        DestroyEnemy,
        AcquireItem,
        Interact,
    }

    [System.Serializable]
    public class QuestObjective
    {
        public QuestType type;
        public int targetId;
        public string reference;
        public int completionCount;
        public string description;


        public QuestObjective() { }

        public QuestObjective(QuestObjective other)
        {
            type = other.type;
            targetId = other.targetId;
            reference = other.reference;
            completionCount = other.completionCount;
            description = other.description;
        }
    }
    #endregion

}
