using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;
using Kame.Define;
using UnityEngine;

namespace Kame.Game.Data
{
    public class DataManager
    {
        public static bool Loaded { get; private set; } = false;
        public static Dictionary<int, Dictionary<int, CharacterStat>> StatDict { get; private set; } = new Dictionary<int, Dictionary<int, CharacterStat>>();
        //public static Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
        public static Dictionary<int, EntityData> EntityDict { get; private set; } = new Dictionary<int, EntityData>();

        public static Dictionary<int, DialogueData> DialogueDict { get; private set; } = new Dictionary<int, DialogueData>();
        public static Dictionary<int, QuestData> QuestDict { get; private set; } = new Dictionary<int, QuestData>();

        public static Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
        
        private static SoundDataLoader _soundDataLoader = null;
        private static EffectDataLoader _effectDataLoader = null;
        public static EffectDataLoader Effect(bool reload = false)
        {
            if(_effectDataLoader == null)
            {
                _effectDataLoader = ScriptableObject.CreateInstance<EffectDataLoader>();
                _effectDataLoader.LoadData();
            }
            return _effectDataLoader;
        }
        public static SoundDataLoader Sound(bool reload = false)
        {
            if(_soundDataLoader == null)
            {
                _soundDataLoader = ScriptableObject.CreateInstance<SoundDataLoader>();
                _soundDataLoader.LoadData();
            }
            return _soundDataLoader;
        }
        
        [RuntimeInitializeOnLoadMethod]
        public static void LoadData()
        {
            StatDict = LoadJson<CharacterStatLoader, int, Dictionary<int, CharacterStat>>("CharacterData").MakeDic();
            SkillDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDic();
            QuestDict = LoadJson<QuestDataLoader, int, QuestData>("QuestData").MakeDic();
            ItemDict = LoadJson<ItemDataLoader, int, ItemData>("ItemData").MakeDic();
            EntityDict = LoadJson<EntityDataLoader, int, EntityData>("EntityData").MakeDic();
            Loaded = true;
        }
        
        public static bool TryGetStat(int id, int level, out CharacterStat stat)
        {
            if (!Loaded)
                LoadData();
            
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

        public static GameEntityType GetEntityType(int entityId)
        {
            int typeInt = (entityId / 10000000) * 10000000;
            if (typeInt == (int) GameEntityType.Player)
                return GameEntityType.Player;
            else if (typeInt == (int) GameEntityType.Object)
                return GameEntityType.Object;
            else if (typeInt == (int) GameEntityType.NPC)
                return GameEntityType.NPC;
            else if (typeInt == (int) GameEntityType.Monster)
                return GameEntityType.Monster;
            return GameEntityType.None;
        }

        public static Loader LoadJson<Loader, Key, Value>(string path) where Loader : IDataLoader<Key, Value>
        {
            TextAsset text = Resources.Load<TextAsset>($"Data/{path}");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text.text);
        }
    }
}
