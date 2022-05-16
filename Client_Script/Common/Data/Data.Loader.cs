using Kame.Game.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kame.Game.Data
{
    public class LoaderUtil
    {
        public static Dictionary<int, T> ConvertListToDic<T>(List<T> list) where T : IManagedData, new()
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
        public List<CharacterStat> list;

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

        public int AddData(string newName)
        {
            throw new NotImplementedException();
        }

        public void RemoveData(int index)
        {
            throw new NotImplementedException();
        }

        public void Copy(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class SkillDataLoader : IDataLoader<int, SkillData>
    {
        public List<SkillData> list;
        public Dictionary<int, SkillData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }

        public int AddData(string newName)
        {
            SkillData skill = new SkillData();
            skill.name = newName;
            list.Add(skill);
            return list.Count;
        }

        public void RemoveData(int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            list.RemoveAt(index);
        }

        public void Copy(int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            SkillData copy = new SkillData(list[index]);
            copy.name = copy.name + "_copy";
            list.Add(copy);
        }
    }
    #endregion

    public class ItemDataLoader : IDataLoader<int, ItemData>
    {
        public List<ItemData> list;
        public Dictionary<int, ItemData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }

        public int AddData(string newName)
        {
            throw new NotImplementedException();
        }

        public void RemoveData(int index)
        {
            throw new NotImplementedException();
        }

        public void Copy(int index)
        {
            throw new NotImplementedException();
        }
    }
    
    public class EntityDataLoader : IDataLoader<int, EntityData>
    {
        public List<EntityData> list;
        public Dictionary<int, EntityData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }

        public int AddData(string newName)
        {
            throw new NotImplementedException();
        }

        public void RemoveData(int index)
        {
            throw new NotImplementedException();
        }

        public void Copy(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class DialogueDataLoader : IDataLoader<int, DialogueData>
    {
        public List<DialogueData> list;

        public Dictionary<int, DialogueData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }

        public int AddData(string newName)
        {
            throw new NotImplementedException();
        }

        public void RemoveData(int index)
        {
            throw new NotImplementedException();
        }

        public void Copy(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class QuestDataLoader : IDataLoader<int, QuestData>
    {
        public List<QuestData> list;

        public Dictionary<int, QuestData> MakeDic()
        {
            return LoaderUtil.ConvertListToDic(list);
        }

        public int AddData(string newName)
        {
            QuestData quest = new QuestData();
            quest.reference = newName;
            list.Add(quest);
            return list.Count;
        }

        public void RemoveData(int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            list.RemoveAt(index);
        }

        public void Copy(int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            QuestData copy = new QuestData(list[index]);
            copy.reference = copy.reference + "_copy";
            list.Add(copy);
        }
    }
}
