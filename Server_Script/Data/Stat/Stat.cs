using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace InflearnServer.Game.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Stat
    {
        [JsonProperty]
        protected List<StatValue> list = new List<StatValue>();
        protected Dictionary<StatType, StatValue> dict = new Dictionary<StatType, StatValue>();

        public Dictionary<StatType, StatValue> Dict => dict;
        public Stat()
        {
        }

        public Stat(Stat other)
        {
            foreach (var pair in other.dict)
            {
                StatValue copy = new StatValue(pair.Value);
                this.dict.Add(pair.Key, copy);
            }
        }

        public Stat ChangeBaseValue(Stat other)
        {
            foreach (var pair in other.Dict)
            {
                ChangeBaseValue(pair.Key, pair.Value.value.BaseValue);
            }
            return this;
        }

        public void ChangeBaseValue(StatType type, float value)
        {
            if (dict.ContainsKey(type))
            {
                dict[type].value.BaseValue = value;
            }
            else
            {
                dict.Add(type, new StatValue(type, value));
            }
        }

        public void RegisterStatModifiedEvent(Action<StatValue> action)
        {
            foreach (var v in dict.Values)
            {
                v.RegisterModEvent(action);
            }
        }

        public void UnRegisterStatModifiedEvent(Action<StatValue> action)
        {
            foreach (var v in dict.Values)
            {
                v.UnRegisterModEvent(action);
            }
        }

        public StatValue this[StatType type]
        {
            get
            {
                if (dict.TryGetValue(type, out StatValue v))
                {
                    return v;
                }
                return null;
            }
        }

        public float GetModifiedValue(StatType type)
        {
            if (dict.TryGetValue(type, out StatValue statValue))
            {
                return statValue.value.ModifiedValue;
            }
            return 0;
        }

        public float GetBaseValue(StatType type)
        {
            if (dict.TryGetValue(type, out StatValue statValue))
            {
                return statValue.value.BaseValue;
            }
            return 0;
        }

        public bool AddModifier(StatType type, IModifier<float> modifier)
        {
            if (dict.TryGetValue(type, out StatValue statValue))
            {
                statValue.value.AddModifier(modifier);
                return true;
            }
            return false;
        }

        public bool RemoveModifier(StatType type, IModifier<float> modifier)
        {
            if (dict.TryGetValue(type, out StatValue statValue))
            {
                statValue.value.RemoveModifier(modifier);
                return true;
            }
            return false;
        }

        [OnSerializing]
        public void OnBeforeSerializeStat(StreamingContext context)
        {
            Dict2List();
        }

        [OnDeserialized]
        public void OnAfterDeserializeStat(StreamingContext context)
        {
            List2Dict();
        }

        private void List2Dict()
        {
            dict.Clear();

            if (list.Count > 1)
            {
                if (list[list.Count - 1].type == list[list.Count - 2].type)
                    list[list.Count - 1].type = StatType.None;
            }

            foreach (var value in list)
            {
                dict[value.type] = value;
            }
            list.Clear();
        }

        private void Dict2List()
        {
            list.Clear();
            foreach (var value in dict.Values)
            {
                list.Add(value);
            }
        }
    }
}

