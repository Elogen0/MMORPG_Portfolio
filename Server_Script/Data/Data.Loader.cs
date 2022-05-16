using InflearnServer.Game.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game.Data
{
    public class LoaderUtil
    {
        public static Dictionary<int, T> ConvertListToDic<T>(T[] list) where T : IManagedData, new()
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();
            foreach (var value in list)
            {
                dict.Add(value.ID, value);
            }
            return dict;
        }
    }

    #region Stat
    public class CharacterStatLoader : IDataLoader<int, Dictionary<int, CharacterStat>>
    {
        public CharacterStat[] list;

        public Dictionary<int, Dictionary<int, CharacterStat>> MakeDic()
        {
            Dictionary<int, Dictionary<int, CharacterStat>> dictionary =
                new Dictionary<int, Dictionary<int, CharacterStat>>();
            foreach (var characterStat in list)
            {
                if (!dictionary.ContainsKey(characterStat.id))
                {
                    dictionary.Add(characterStat.id, new Dictionary<int, CharacterStat>());
                }
                dictionary[characterStat.id].Add(characterStat.level, characterStat);
                characterStat.Hp = characterStat.MaxHp;
            }
            return dictionary;
        }
    }

    public class SkillDataLoader : IDataLoader<int, SkillData>
    {
        public SkillData[] list;
        public Dictionary<int, SkillData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }
    }
    #endregion

    public class ItemDataLoader : IDataLoader<int, ItemData>
    {
        public ItemData[] list;
        public Dictionary<int, ItemData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }
    }
    public class MonsterDataLoader : IDataLoader<int, EntityData>
    {
        public EntityData[] list;
        public Dictionary<int, EntityData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }
    }

    public class QuestDataLoader : IDataLoader<int, QuestData>
    {
        public QuestData[] list;

        public Dictionary<int, QuestData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }
    }
}
