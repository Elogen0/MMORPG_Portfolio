using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace InflearnServer.Game.Data
{
    class DataManager
    {
        public static Dictionary<int, Dictionary<int, CharacterStat>> StatDict { get; private set; } = new Dictionary<int, Dictionary<int, CharacterStat>>();
        public static Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
        public static Dictionary<int, EntityData> MonsterDict { get; private set; } = new Dictionary<int, EntityData>();
        public static Dictionary<int, QuestData> QuestDict { get; private set; } = new Dictionary<int, QuestData>();
        public static void LoadData()
        {
            StatDict = LoadJson<CharacterStatLoader, int, Dictionary<int, CharacterStat>>("CharacterData").MakeDic();
            SkillDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDic();
            ItemDict = LoadJson<ItemDataLoader, int, ItemData>("ItemData").MakeDic();
            MonsterDict = LoadJson<MonsterDataLoader, int, EntityData>("EntityData").MakeDic();
            QuestDict = LoadJson<QuestDataLoader, int, QuestData>("QuestData").MakeDic();
        }

        public static bool TryGetStat(int id, int level, out CharacterStat stat)
        {
            if (StatDict == null)
            {
                LoadData();
            }
            if (StatDict.TryGetValue(id, out Dictionary<int, CharacterStat> charStatDict))
            {
                if (charStatDict.TryGetValue(level, out CharacterStat retStat))
                {
                    stat = retStat;
                    return true;
                }
            }
            stat = null;
            return false;
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : IDataLoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }

        
    }
}
